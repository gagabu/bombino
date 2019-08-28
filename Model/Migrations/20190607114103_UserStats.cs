using Microsoft.EntityFrameworkCore.Migrations;

namespace BombinoBomberBot.Model.Migrations
{
    public partial class UserStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Trolls",
                table: "Rooms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserStats",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Wins = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => new { x.RoomId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserStats_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStats_RoomId",
                table: "UserStats",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStats_UserId",
                table: "UserStats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStats");

            migrationBuilder.DropColumn(
                name: "Trolls",
                table: "Rooms");
        }
    }
}
