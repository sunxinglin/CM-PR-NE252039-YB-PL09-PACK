using FutureTech.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsZero.Core.Entities
{

    /// <summary>
    /// User-ClaimEntity映射表
    /// </summary>
    [Table("UserClaims")]
    public class UserClaim : FutureBaseEntity<int>
    { 
        public int UserId { get; set; }
        public int ClaimEntityId { get; set; }
        public User User { get; set; }
        public ClaimEntity ClaimEntity { get; set; }
    }

    internal class UserClaimEntityConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasOne(uc => uc.User).WithMany(u => u.UserClaims).HasForeignKey(uc => uc.UserId).IsRequired();
            builder.HasOne(uc => uc.ClaimEntity)
                .WithMany(u => u.UserClaims)
                .HasForeignKey(uc => uc.ClaimEntityId)
                .IsRequired();
        }
    }

}
