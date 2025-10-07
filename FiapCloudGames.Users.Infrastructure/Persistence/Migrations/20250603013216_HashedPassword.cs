using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGames.Users.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HashedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2y$10$0nJ0qFKXIeecwowh7sg53./xAESh24u0mM.NdJTT1b/TJ3FnS7Pym");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2y$10$K1UygvGpjh9Mfw1z3PZemOQ8ltbEiZ5lHh8ugC9PcOa5wSKtount.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "senha1234$$");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "senha1234$$");
        }
    }
}
