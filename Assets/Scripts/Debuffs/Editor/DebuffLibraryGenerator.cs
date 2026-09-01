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
                    "Урон +35%, но здоровье -30%.", "Damage +35%, but health -30%.", "Hasar +35, can -30.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Power, 1.35f),
                        new DebuffModifier(PerkType.MaxHp, 0.70f),
                    }),

                new Definition("Debuff_Berserk", "Берсерк", "Berserk", "Berserk",
                    "Скорость атаки +30%, но броня -35%.", "Attack speed +30%, but armor -35%.", "Saldiri +30, zirh -35.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 0.70f),
                        new DebuffModifier(PerkType.Armor, 0.65f),
                    }),

                new Definition("Debuff_Marathoner", "Марафонец", "Marathoner", "Maratoncu",
                    "Скорость +25%, но урон -25%.", "Speed +25%, but damage -25%.", "Hiz +25, hasar -25.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Speed, 1.25f),
                        new DebuffModifier(PerkType.Power, 0.75f),
                    }),

                new Definition("Debuff_Vitality", "Живучесть", "Vitality", "Canlilik",
                    "Здоровье +35%, но реген -60%.", "Health +35%, but regen -60%.", "Can +35, yenilenme -60.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.MaxHp, 1.35f),
                        new DebuffModifier(PerkType.HpRegeneration, 0.40f),
                    }),

                new Definition("Debuff_Regenerator", "Регенератор", "Regenerator", "Yenileyici",
                    "Реген +120%, но здоровье -25%.", "Regen +120%, but health -25%.", "Yenilenme +120, can -25.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.HpRegeneration, 2.20f),
                        new DebuffModifier(PerkType.MaxHp, 0.75f),
                    }),

                new Definition("Debuff_Turtle", "Черепаха", "Turtle", "Kaplumbaga",
                    "Броня +45% и здоровье +15%, но скорость -35%.",
                    "Armor +45% and health +15%, but speed -35%.",
                    "Zirh +45, can +15, hiz -35.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Armor, 1.45f),
                        new DebuffModifier(PerkType.MaxHp, 1.15f),
                        new DebuffModifier(PerkType.Speed, 0.65f),
                    }),

                new Definition("Debuff_Duelist", "Дуэлянт", "Duelist", "Duellocu",
                    "Скорость атаки +35% и скорость +15%, но здоровье -35%.",
                    "Attack speed +35% and speed +15%, but health -35%.",
                    "Saldiri +35, hiz +15, can -35.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 0.65f),
                        new DebuffModifier(PerkType.Speed, 1.15f),
                        new DebuffModifier(PerkType.MaxHp, 0.65f),
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
