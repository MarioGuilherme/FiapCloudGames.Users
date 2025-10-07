using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Users.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(60);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(60);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(60);

        // Dados de login de administrador e de usuário comum
        builder.HasData([
            new(1, "Usuário Administrador", "adm@gmail.com", "$2y$10$0nJ0qFKXIeecwowh7sg53./xAESh24u0mM.NdJTT1b/TJ3FnS7Pym", UserType.Admin), // Senha: senha1234$$
            new(2, "Usuário Comum", "comum@gmail.com", "$2y$10$K1UygvGpjh9Mfw1z3PZemOQ8ltbEiZ5lHh8ugC9PcOa5wSKtount.", UserType.User) // Senha: senha1234$$
        ]);
    }
}
