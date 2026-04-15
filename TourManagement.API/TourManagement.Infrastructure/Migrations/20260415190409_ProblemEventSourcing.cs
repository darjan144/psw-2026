using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProblemEventSourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Problems");

            migrationBuilder.CreateTable(
                name: "ProblemStateEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProblemId = table.Column<long>(type: "bigint", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CausedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    EventType = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemStateEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemStateEvents_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemStateEvents_ProblemId",
                table: "ProblemStateEvents",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemStateEvents_ProblemId_SequenceNumber",
                table: "ProblemStateEvents",
                columns: new[] { "ProblemId", "SequenceNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemStateEvents");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Problems",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
