using System.Collections.Generic;
using System.Linq;

using Common.Template;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterEntitySystems
{
public class CharacterEntityHub : SingletonHub<CharacterEntityHub,CharacterEntity>
{
    [Title("Methods")]
    public IEnumerable<PcEntity> AllPcEntitiesPy => this._instanceId_instance.Values.OfType<PcEntity>();
    public IEnumerable<NpcEntity> AllNpcEntitiesPy => this._instanceId_instance.Values.OfType<NpcEntity>();

    public void Initialize()
    {
        this._instanceId_instance = new Dictionary<int,CharacterEntity>(50);
    }
}
}