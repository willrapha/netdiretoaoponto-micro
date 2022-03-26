using DevFreela.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFreela.Infrastructure.Persistence.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
               .HasKey(p => p.Id);

            builder
                .HasMany(p => p.Skills)
                .WithOne()
                .HasForeignKey(u => u.IdSkill)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
