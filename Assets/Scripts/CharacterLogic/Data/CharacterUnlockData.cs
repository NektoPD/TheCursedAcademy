using System.Collections.Generic;

namespace CharacterLogic.Data
{
    public class CharacterUnlockData
    {
        public Dictionary<CharacterData.CharacterType, bool> Data = new()
        {
            { CharacterData.CharacterType.Girl1, true }, { CharacterData.CharacterType.Girl2, false },
            { CharacterData.CharacterType.Girl3, false }
        };
    }
}