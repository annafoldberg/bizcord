using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChannelService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSagaState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBanSagaStates",
                columns: table => new
                {
                    SagaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BanSucceeded = table.Column<bool>(type: "boolean", nullable: false),
                    MembershipsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    MembershipsRestored = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsFailed = table.Column<bool>(type: "boolean", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBanSagaStates", x => x.SagaId);
                });

            migrationBuilder.CreateTable(
                name: "UserBanSagaMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SagaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBanSagaMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBanSagaMemberships_UserBanSagaStates_SagaId",
                        column: x => x.SagaId,
                        principalTable: "UserBanSagaStates",
                        principalColumn: "SagaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBanSagaMemberships_SagaId_ChannelId",
                table: "UserBanSagaMemberships",
                columns: new[] { "SagaId", "ChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBanSagaMemberships");

            migrationBuilder.DropTable(
                name: "UserBanSagaStates");
        }
    }
}
