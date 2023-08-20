using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.Domain;

public class JWTSettings
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string IssuerSigningKey { get; set; }
    public int AccessTokenLifetimeInHours { get; set; }
    public int RefreshTokenLifetimeInHours { get; set; }
}

