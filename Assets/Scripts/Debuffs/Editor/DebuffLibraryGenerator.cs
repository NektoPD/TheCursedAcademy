#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Debuffs.EditorTools
{
    public static class DebuffLibraryGenerator
    {
        private const string FolderPath = "Assets/Configs/Debuffs";

        [MenuItem("Tools/Debuffs/Generate Debuff Library")]
        public static void Generate()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Configs"))
                AssetDatabase.CreateFolder("Assets", "Configs");

            if (!AssetDatabase.IsValidFolder(FolderPath))
                AssetDatabase.CreateFolder("Assets/Configs", "Debuffs");

            var definitions = GetDefinitions().ToList();

            foreach (var def in definitions)
            {
                string assetPath = Path.Combine(FolderPath, def.FileName + ".asset");

                var data = ScriptableObject.CreateInstance<DebuffData>();
                data.SetLocalization(def.NameRu, def.NameEn, def.NameTr,
                    def.DescRu, def.DescEn, def.DescTr);
                data.SetModifiers(def.Modifiers);

                AssetDatabase.CreateAsset(data, assetPath);
            }

            RemoveStaleAssets(definitions);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DebuffLibraryGenerator] Generated " + definitions.Count + " debuffs in " + FolderPath +
                ". Assign icons manually in each asset.");
        }

        private static void RemoveStaleAssets(List<Definition> definitions)
        {
            var keptNames = definitions.Select(def => def.FileName + ".asset").ToHashSet();

            foreach (string assetPath in Directory.GetFiles(FolderPath, "*.asset"))
            {
                string fileName = Path.GetFileName(assetPath);

                if (keptNames.Contains(fileName))
                    continue;

                AssetDatabase.DeleteAsset(assetPath);
                Debug.Log("[DebuffLibraryGenerator] Removed stale debuff asset " + fileName);
            }
        }

        private static IEnumerable<Definition> GetDefinitions()
        {
            return new List<Definition>
            {
                new Definition("Debuff_GlassCannon", "Стеклянная пушка", "Glass Cannon", "Cam Top",
                    "Урон +20%, но здоровье -15%.",
                    "Damage +20%, but health -15%.",
                    "Hasar +%20, ancak can -%15.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Power, 1.20f),
                        new DebuffModifier(PerkType.MaxHp, 0.85f),
                    }),

                new Definition("Debuff_Berserk", "Берсерк", "Berserk", "Berserk",
                    "Скорость атаки +10%, но броня -10%.",
                    "Attack speed +10%, but armor -10%.",
                    "Saldırı hızı +%10, ancak zırh -%10.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 1f / 1.10f),
                        new DebuffModifier(PerkType.Armor, 0.90f),
                    }),

                new Definition("Debuff_Marathoner", "Марафонец", "Marathoner", "Maratoncu",
                    "Скорость передвижения +10%, но урон -10%.",
                    "Movement speed +10%, but damage -10%.",
                    "Hareket hızı +%10, ancak hasar -%10.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Speed, 1.10f),
                        new DebuffModifier(PerkType.Power, 0.90f),
                    }),

                new Definition("Debuff_Vitality", "Живучесть", "Vitality", "Canlilik",
                    "Здоровье +20%, но реген -25%.",
                    "Health +20%, but regen -25%.",
                    "Can +%20, ancak yenilenme -%25.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.MaxHp, 1.20f),
                        new DebuffModifier(PerkType.HpRegeneration, 0.75f),
                    }),

                new Definition("Debuff_Regenerator", "Регенератор", "Regenerator", "Yenileyici",
                    "Реген +40%, но здоровье -12%.",
                    "Regen +40%, but health -12%.",
                    "Yenilenme +%40, ancak can -%12.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.HpRegeneration, 1.40f),
                        new DebuffModifier(PerkType.MaxHp, 0.88f),
                    }),

                new Definition("Debuff_Turtle", "Черепаха", "Turtle", "Kaplumbaga",
                    "Броня +20% и здоровье +10%, но скорость передвижения -10%.",
                    "Armor +20% and health +10%, but movement speed -10%.",
                    "Zırh +%20 ve can +%10, ancak hareket hızı -%10.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Armor, 1.20f),
                        new DebuffModifier(PerkType.MaxHp, 1.10f),
                        new DebuffModifier(PerkType.Speed, 0.90f),
                    }),

                new Definition("Debuff_Duelist", "Дуэлянт", "Duelist", "Duellocu",
                    "Скорость атаки +8% и скорость передвижения +5%, но здоровье -12%.",
                    "Attack speed +8% and movement speed +5%, but health -12%.",
                    "Saldırı hızı +%8 ve hareket hızı +%5, ancak can -%12.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 1f / 1.08f),
                        new DebuffModifier(PerkType.Speed, 1.05f),
                        new DebuffModifier(PerkType.MaxHp, 0.88f),
                    }),
            };
        }

        private struct Definition
        {
            public string FileName;
            public string NameRu;
            public string NameEn;
            public string NameTr;
            public string DescRu;
            public string DescEn;
            public string DescTr;
            public List<DebuffModifier> Modifiers;

            public Definition(string fileName, string nameRu, string nameEn, string nameTr,
                string descRu, string descEn, string descTr, List<DebuffModifier> modifiers)
            {
                FileName = fileName;
                NameRu = nameRu;
                NameEn = nameEn;
                NameTr = nameTr;
                DescRu = descRu;
                DescEn = descEn;
                DescTr = descTr;
                Modifiers = modifiers;
            }
        }
    }
}
#endif
