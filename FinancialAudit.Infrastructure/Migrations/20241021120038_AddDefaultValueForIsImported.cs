using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialAudit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultValueForIsImported : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImported",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImported",
                table: "Transactions");
        }
    }
}
