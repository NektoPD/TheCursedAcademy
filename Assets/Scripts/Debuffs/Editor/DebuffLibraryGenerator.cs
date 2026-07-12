#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
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

            foreach (var def in GetDefinitions())
            {
                string assetPath = Path.Combine(FolderPath, def.FileName + ".asset");

                var data = ScriptableObject.CreateInstance<DebuffData>();
                data.SetLocalization(def.NameRu, def.NameEn, def.NameTr,
                    def.DescRu, def.DescEn, def.DescTr);
                data.SetModifiers(def.Modifiers);

                AssetDatabase.CreateAsset(data, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DebuffLibraryGenerator] Generated 10 debuffs in " + FolderPath +
                ". Assign icons manually in each asset.");
        }

        private static IEnumerable<Definition> GetDefinitions()
        {
            return new List<Definition>
            {
                new Definition("Debuff_GlassCannon", "Стеклянная пушка", "Glass Cannon", "Cam Top",
                    "Урон +40%, но здоровье -25%.", "Damage +40%, but health -25%.", "Hasar +40, can -25.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Power, 1.40f),
                        new DebuffModifier(PerkType.MaxHp, 0.75f),
                    }),

                new Definition("Debuff_HeavyArmor", "Тяжёлая броня", "Heavy Armor", "Agir Zirh",
                    "Броня +50%, но скорость -20%.", "Armor +50%, but speed -20%.", "Zirh +50, hiz -20.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Armor, 1.50f),
                        new DebuffModifier(PerkType.Speed, 0.80f),
                    }),

                new Definition("Debuff_Berserk", "Берсерк", "Berserk", "Berserk",
                    "Скорость атаки +35%, но броня -30%.", "Attack speed +35%, but armor -30%.", "Saldiri +35, zirh -30.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 0.65f),
                        new DebuffModifier(PerkType.Armor, 0.70f),
                    }),

                new Definition("Debuff_Marathoner", "Марафонец", "Marathoner", "Maratoncu",
                    "Скорость +30%, но урон -20%.", "Speed +30%, but damage -20%.", "Hiz +30, hasar -20.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Speed, 1.30f),
                        new DebuffModifier(PerkType.Power, 0.80f),
                    }),

                new Definition("Debuff_Vampire", "Живучесть", "Vitality", "Canlilik",
                    "Здоровье +40%, но реген -50%.", "Health +40%, but regen -50%.", "Can +40, yenilenme -50.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.MaxHp, 1.40f),
                        new DebuffModifier(PerkType.HpRegeneration, 0.50f),
                    }),

                new Definition("Debuff_Regenerator", "Регенератор", "Regenerator", "Yenileyici",
                    "Реген +80%, но здоровье -20%.", "Regen +80%, but health -20%.", "Yenilenme +80, can -20.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.HpRegeneration, 1.80f),
                        new DebuffModifier(PerkType.MaxHp, 0.80f),
                    }),

                new Definition("Debuff_Sniper", "Снайпер", "Sniper", "Keskin Nisanci",
                    "Урон +50%, но скорость атаки -25%.", "Damage +50%, but attack speed -25%.", "Hasar +50, saldiri -25.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Power, 1.50f),
                        new DebuffModifier(PerkType.AttackCooldown, 1.25f),
                    }),

                new Definition("Debuff_Turtle", "Черепаха", "Turtle", "Kaplumbaga",
                    "Броня +60% и здоровье +20%, но скорость -35%.",
                    "Armor +60% and health +20%, but speed -35%.",
                    "Zirh +60, can +20, hiz -35.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Armor, 1.60f),
                        new DebuffModifier(PerkType.MaxHp, 1.20f),
                        new DebuffModifier(PerkType.Speed, 0.65f),
                    }),

                new Definition("Debuff_Duelist", "Дуэлянт", "Duelist", "Duellocu",
                    "Скорость атаки +40% и скорость +15%, но здоровье -30%.",
                    "Attack speed +40% and speed +15%, but health -30%.",
                    "Saldiri +40, hiz +15, can -30.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.AttackCooldown, 0.60f),
                        new DebuffModifier(PerkType.Speed, 1.15f),
                        new DebuffModifier(PerkType.MaxHp, 0.70f),
                    }),

                new Definition("Debuff_Reckless", "Безрассудный", "Reckless", "Pervasiz",
                    "Урон +30% и скорость +20%, но броня -40%.",
                    "Damage +30% and speed +20%, but armor -40%.",
                    "Hasar +30, hiz +20, zirh -40.",
                    new List<DebuffModifier>
                    {
                        new DebuffModifier(PerkType.Power, 1.30f),
                        new DebuffModifier(PerkType.Speed, 1.20f),
                        new DebuffModifier(PerkType.Armor, 0.60f),
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
