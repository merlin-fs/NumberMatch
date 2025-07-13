using Game.Core.Data;
using Unity.Collections;

namespace Game.Core
{
    /*
    public class Finder
    {
        public static NativeArray<MergeCandidate> FindAllPossibleMerges(FieldData field, MergeRuleSet mergeRules)
        {
            var result = new NativeList<MergeCandidate>();
            var width = field.Size.x;
            var height = field.Size.y;

            for (var y1 = 0; y1 < height; y1++)
            for (var x1 = 0; x1 < width; x1++)
            {
                var cellA = field[y1, x1];
                if (cellA.IsEmpty) continue;

                for (var y2 = 0; y2 < height; y2++)
                for (var x2 = 0; x2 < width; x2++)
                {
                    if (y1 == y2 && x1 == x2) continue; // не одна й та сама клітинка
                    var cellB = field[y2, x2];
                    if (cellB.IsEmpty) continue;

                    // Спочатку перевіряємо саме правило об’єднання чисел:
                    if (!(cellA.Value == cellB.Value || cellA.Value + cellB.Value == 10))
                        continue;

                    // Далі — правила позицій
                    if (mergeRules.CanMerge(field, y1, x1, y2, x2))
                        result.Add(new MergeCandidate(y1, x1, y2, x2));
                }
            }

            return result.ToArray(Allocator.Persistent);
        }
    }
    */
}