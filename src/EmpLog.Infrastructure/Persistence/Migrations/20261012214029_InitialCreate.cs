using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpLog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Motoristas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    CnhNumero = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    CnhCategoria = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    CnhValidade = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motoristas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Motoristas_CnhNumero",
                table: "Motoristas",
                column: "CnhNumero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motoristas_Cpf",
                table: "Motoristas",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Motoristas");
        }
    }
}
