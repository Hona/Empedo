using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Empedo.Discord.Commands
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class EmpedoModuleBase : BaseCommandModule
    {
        public ILogger Logger { get; set; }
    }
}