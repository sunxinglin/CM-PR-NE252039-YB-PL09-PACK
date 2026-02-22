using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsZero.Core.Entities
{
    /// <summary>
    /// ApiKey 的类型
    /// </summary>
    public enum ApiKeyType { Substation = 1, Admin = 2 }

    [Table("ApiKey")]
    public class ApiKey
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 标记该密钥是发布给谁的
        /// </summary>
        /// <value></value>
        [Column("user_identifier")]
        [MaxLength(255)]
        public string ClientIdentifier { get; set; }

        /// <summary>
        /// 该密钥的令牌
        /// </summary>
        /// <value></value>
        [Column("token")]
        [MaxLength(255)]
        public string Token { get; set; }

        // 对该密钥的描述
        [Column("description")]
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 是否有效，默认有效
        /// </summary>
        /// <value></value>
        [Column("valid")]
        public bool Valid { get; set; } = true;

        /// <summary>
        /// ApiKey 的类型，比如是采集站、还是管理员
        /// </summary>
        [Column("api_key_type")]
        public ApiKeyType ApiKeyType { get; set; }

    }

    public class ClientIdDefines
    {
        public const string Shared   = "Shared";
        public const string Section1 = "Section1";
        public const string Section2 = "Section2";
        public const string Section3 = "Section3";
        public const string SectionX = "SectionX";
    }

    public class ApiKeyEntityTypeConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            // 根据 clientId + token 查询
            builder.HasIndex(ak => new { ak.ClientIdentifier, ak.Token, ak.Valid, ak.ApiKeyType});

            builder.HasData(new ApiKey
            {
                Id = 1,
                ApiKeyType = ApiKeyType.Substation,
                ClientIdentifier = ClientIdDefines.Shared,
                Token = "123456",
                Valid = true,
                Description = string.Empty,
            });

            builder.HasData(new ApiKey { 
                Id = 2, 
                ApiKeyType = ApiKeyType.Substation,
                ClientIdentifier = ClientIdDefines.Section1,
                Token = "123456",
                Valid = true,
                Description = string.Empty,
            });

            builder.HasData(new ApiKey { 
                Id = 3, 
                ApiKeyType = ApiKeyType.Substation,
                ClientIdentifier = ClientIdDefines.Section2,
                Token = "123456",
                Valid = true,
                Description = string.Empty,
            });

            builder.HasData(new ApiKey { 
                Id = 4, 
                ApiKeyType = ApiKeyType.Substation,
                ClientIdentifier = ClientIdDefines.Section3,
                Token = "123456",
                Valid = true,
                Description = string.Empty,
            });

            builder.HasData(new ApiKey { 
                Id = 5, 
                ApiKeyType = ApiKeyType.Substation,
                ClientIdentifier = ClientIdDefines.SectionX,
                Token = "123456",
                Valid = true,
                Description = string.Empty,
            });

        }
    }
}
