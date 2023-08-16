using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.DAL.Entities
{
    
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }

        public List<string> PermissionIds { get; set; }

        public override string PartitionKey { get; set; } = nameof(Role);

    }
}
