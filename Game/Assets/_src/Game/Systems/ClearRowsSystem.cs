using System.Collections.Generic;
using System.Linq;
using Game.Core.Components;
using TFs.Common.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Core.Systems
{
    public class ClearRowsSystem : ISystem
    {
        private readonly Entity _fieldEntity;

        public ClearRowsSystem(Entity fieldEntity)
        {
            _fieldEntity = fieldEntity;
        }

        public void OnCreate(EntityManager manager, SystemQuery query)
        {
            query.AddComponentType<MergeRequestComponent>();
        }

        public void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime)
        {
            var field = manager.GetComponent<FieldComponent>(_fieldEntity);
            var cells = manager.GetBuffer<CellComponent>(_fieldEntity);

            int width = field.Size.x;
            int height = field.Size.y;

            // Копіюємо дані для Job
            var cellArray = new NativeArray<CellComponent>(cells.AsArray(), Allocator.TempJob);
            var rowIsEmpty = new NativeArray<bool>(height, Allocator.TempJob);

            // Запускаємо Job
            var job = new CheckEmptyRowsJob
            {
                Cells = cellArray,
                Width = width,
                Height = height,
                RowIsEmpty = rowIsEmpty
            };

            var handle = job.Schedule(height, 1);
            handle.Complete();

            // Збираємо всі номери порожніх рядків
            var rowsToRemove = new List<int>();
            for (int y = 0; y < height; y++)
                if (rowIsEmpty[y]) rowsToRemove.Add(y);

            cellArray.Dispose();
            rowIsEmpty.Dispose();

            if (rowsToRemove.Count == 0) return;

            // --- Очищення (на головному потоці) ---
            foreach (int row in rowsToRemove.OrderByDescending(r => r))
            {
                // Видаляємо клітинки цього рядка
                int startIdx = row * width;
                for (int i = 0; i < width; i++)
                    cells.RemoveAt(startIdx); // Важливо: завжди видаляти з однакового startIdx

                // Зсуваємо всі клітинки вище (оновлюємо Index)
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var pos = field.FromIndex(cell.Index);
                    if (pos.y >= row) continue;
                    // Зсуваємо клітинку вниз
                    pos.y += 1;
                    cell.Index = field.At(pos.x, pos.y);
                    cells[i] = cell;
                }
            }

            // Оновлюємо розмір поля
            field.Size = new int2(width, height - rowsToRemove.Count);
            manager.UpdateComponent(_fieldEntity, field);
        }

        [BurstCompile]
        private struct CheckEmptyRowsJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<CellComponent> Cells;
            [WriteOnly] public NativeArray<bool> RowIsEmpty;
            public int Width;
            public int Height;

            public void Execute(int y)
            {
                var empty = true;
                var startIdx = y * Width;
                for (var x = 0; x < Width; x++)
                {
                    if (Cells[startIdx + x].IsRemoved) continue;
                    empty = false;
                    break;
                }

                RowIsEmpty[y] = empty;
            }
        }

    }
}