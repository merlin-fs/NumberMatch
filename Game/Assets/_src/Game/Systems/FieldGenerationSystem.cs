using System.Collections.Generic;
using System.Linq;
using Game.Configs;
using Game.Core.Components;
using TFs.Common.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Core.Systems
{
    public class FieldGenerationSystem : ISystem
    {
        private readonly FieldGenerationConfig _config;
        private readonly Entity _fieldEntity; // Сутність поля
        
        public FieldGenerationSystem(FieldGenerationConfig config, Entity fieldEntity)
        {
            _config = config;
            _fieldEntity = fieldEntity;
        }
        
        public void OnCreate(EntityManager manager, SystemQuery query)
        {
            query.AddComponentType<AddNumbersRequestComponent>();
        }

        public void OnUpdate(EntityManager manager, NativeArray<Entity> entities, float deltaTime)
        {
            if (entities.Length == 0) return;
            
            var field = manager.GetComponent<FieldComponent>(_fieldEntity);
            var cells = manager.GetBuffer<CellComponent>(_fieldEntity);
            var deck = manager.GetBuffer<DeckComponent>(_fieldEntity);
            var max = field.At(field.Size.x, field.Size.y);
            
            foreach (var entity in entities)
            {
                var request = manager.GetComponent<AddNumbersRequestComponent>(entity);
                AddNumbers(deck, cells, request.NumbersCount, request.Index);
                
                var lastIndex = request.Index + request.NumbersCount - 1;
                var neededRows = (lastIndex / field.Size.x) + 1;
                field.Size.y = math.max(field.Size.y, neededRows);
                manager.UpdateComponent(_fieldEntity, field);
                manager.RemoveEntity(entity);
            }
        }
        
        private void AddNumbers(NativeList<DeckComponent> desk, NativeList<CellComponent> cells, int numbersCount, int startIndex)
        {
            cells.Capacity += numbersCount; // Збільшуємо ємність буфера
            
            for (var i = 0; i < numbersCount; i++)
            {
                if (desk.Length == 0)
                {
                    var deck = BuildDeck(_config.Sum10PairsPerType, _config.EqualPairsPerType, _config.Seed).Reinterpret<DeckComponent>();
                    desk.AddRange(deck);
                    deck.Dispose();
                }
                
                var value = desk[0].Value;
                desk.RemoveAt(0);
                cells.Add(new CellComponent
                {
                    Index = startIndex + i,
                    Value = value,
                    IsRemoved = false,
                });
            }
        }
        
        private NativeArray<int> BuildDeck(int sum10PairsPerType, int equalPairsPerType, int seed, int chunkSize = 18, int chaosRounds = 5)
        {
            
            var sum10Pairs = new List<(int, int)> { (1,9), (2,8), (3,7), (4,6) };
            var equalPairs = new List<(int, int)> { (1,1), (2,2), (3,3), (4,4), (5,5), (6,6), (7,7), (8,8), (9,9) };

            var rng = seed != 0 ? new System.Random(seed) : new System.Random();

            // Створюємо колоду з пар
            var deck = new List<int>();

            // Додаємо “десяткові” пари в 3 рази більше
            foreach (var pair in sum10Pairs)
                for (int i = 0; i < sum10PairsPerType; i++)
                {
                    if (rng.Next(2) == 0)
                    {
                        deck.Add(pair.Item1);
                        deck.Add(pair.Item2);
                    }
                    else
                    {
                        deck.Add(pair.Item2);
                        deck.Add(pair.Item1);
                    }
                }            
            
            // Додаємо однакові пари
            foreach (var pair in equalPairs)
                for (int i = 0; i < equalPairsPerType; i++)
                {
                    deck.Add(pair.Item1);
                    deck.Add(pair.Item2);
                }            
            
            // Chaos rounds
            for (int chaos = 0; chaos < chaosRounds; chaos++)
            {
                // 1. Classic shuffle
                FisherYatesShuffle(deck, rng);

                // 2. Chunk interleave
                var chunks = new List<List<int>>();
                for (int i = 0; i < deck.Count; i += chunkSize)
                    chunks.Add(deck.Skip(i).Take(chunkSize).ToList());

                var newDeck = new List<int>();
                int maxChunkLength = chunks.Max(c => c.Count);
                for (int i = 0; i < maxChunkLength; i++)
                {
                    var indices = Enumerable.Range(0, chunks.Count).OrderBy(_ => rng.Next()).ToList();
                    foreach (int idx in indices)
                        if (i < chunks[idx].Count)
                            newDeck.Add(chunks[idx][i]);
                }

                deck = newDeck;

                // 3. Randomly reverse chunks or halves
                if (rng.Next(2) == 0)
                {
                    // Reverse left or right half
                    int mid = deck.Count / 2;
                    if (rng.Next(2) == 0)
                        deck = deck.Take(mid).Reverse().Concat(deck.Skip(mid)).ToList();
                    else
                        deck = deck.Take(mid).Concat(deck.Skip(mid).Reverse()).ToList();
                }
                else
                {
                    // Reverse random chunk
                    int from = rng.Next(deck.Count / 2);
                    int len = rng.Next(4, chunkSize + 1);
                    deck = deck.Take(from)
                        .Concat(deck.Skip(from).Take(len).Reverse())
                        .Concat(deck.Skip(from + len))
                        .ToList();
                }

                // 4. Recursive chunk shuffle
                deck = RecursiveShuffle(deck, rng, depth: 2);
            }
            // Класична тасовка наприкінці для повного хаосу
            FisherYatesShuffle(deck, rng);

            return new NativeArray<int>(deck.ToArray(), Allocator.Temp);
        }      
        
        private static List<int> RecursiveShuffle(List<int> deck, System.Random rng, int depth)
        {
            if (depth == 0 || deck.Count < 8) return deck.OrderBy(_ => rng.Next()).ToList();
            int mid = deck.Count / 2;
            var left = RecursiveShuffle(deck.Take(mid).ToList(), rng, depth - 1);
            var right = RecursiveShuffle(deck.Skip(mid).ToList(), rng, depth - 1);
            var merged = left.Concat(right).ToList();
            FisherYatesShuffle(merged, rng);
            return merged;
        }

        private static void FisherYatesShuffle<T>(IList<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}