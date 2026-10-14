using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexCart.Domain.Stores;
using NexCart.Persistence.Identity;

namespace NexCart.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Handle).HasMaxLength(40).IsRequired();
        // Handles are stored lowercase, so a plain unique index makes them unique case-insensitively
        builder.HasIndex(s => s.Handle).IsUnique();
        builder.Property(s => s.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(s => s.OwnerAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
