using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.DAL;

[PartitionKeyPath("/name")]
public class User : FullItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public virtual string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
    protected override string GetPartitionKeyValue() => Name;
    
}
