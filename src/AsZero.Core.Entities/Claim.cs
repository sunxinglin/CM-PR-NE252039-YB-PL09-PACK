using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using FutureTech.Dal.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsZero.Core.Entities
{
    [Table("Claim")]
    public class ClaimEntity: FutureBaseEntity<int>
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; } 

        public IList<UserClaim> UserClaims { get; set; }
    }


    internal class ClaimEntityConfiguration : IEntityTypeConfiguration<ClaimEntity>
    {
        public void Configure(EntityTypeBuilder<ClaimEntity> builder)
        {
            builder.HasIndex(e => new {e.ClaimType, e.ClaimValue });
        }
    }

}
