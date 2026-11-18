using CharacterLogic.Data;
using Data;
using PlayerPerksController;
using WalletSystem;

namespace YG
{
    public partial class SavesYG
    {
        public int money = 1;
        public string newPlayerName = "Hello!";
        public bool[] openLevels = new bool[3];

        public PerkDataWrapper PerkDataWrapper = new PerkDataWrapper();
        public WalletWrapper WalletWrapper = new WalletWrapper();
        public bool IsTutorialCompleted;
        public CharacterUnlockData CharacterUnlockDatas = new CharacterUnlockData();


        public SavesYG()
        {
            openLevels[1] = true;
            
        }
    }
}
