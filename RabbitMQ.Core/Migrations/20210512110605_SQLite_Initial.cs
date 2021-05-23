using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrowserTextRPG.Migrations
{
    public partial class SQLite_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Damage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Strength = table.Column<int>(type: "INTEGER", nullable: false),
                    Intelligence = table.Column<int>(type: "INTEGER", nullable: false),
                    Agility = table.Column<int>(type: "INTEGER", nullable: false),
                    charType = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "date('now')"),
                    ModificationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Fights = table.Column<int>(type: "INTEGER", nullable: false),
                    Defeats = table.Column<int>(type: "INTEGER", nullable: false),
                    Victories = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkills",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkills", x => new { x.CharacterId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_CharacterSkills_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapons_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Admin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Player" });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Damage", "Name" },
                values: new object[] { 1, 50, "FireBall" });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Damage", "Name" },
                values: new object[] { 2, 100, "Blizzard" });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Damage", "Name" },
                values: new object[] { 3, 70, "Frenzy" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "PasswordHash", "PasswordSalt", "RoleId" },
                values: new object[] { 1, "Anton", new byte[] { 186, 122, 249, 45, 81, 194, 96, 245, 196, 146, 116, 254, 122, 32, 131, 81, 232, 75, 151, 58, 151, 143, 219, 105, 101, 29, 101, 5, 122, 123, 228, 13, 77, 39, 228, 104, 91, 229, 247, 80, 254, 249, 182, 68, 0, 47, 54, 111, 19, 245, 230, 131, 146, 20, 164, 151, 217, 121, 227, 185, 254, 198, 142, 65 }, new byte[] { 240, 170, 87, 15, 126, 15, 202, 230, 128, 193, 58, 234, 146, 132, 107, 142, 145, 154, 155, 21, 7, 176, 152, 110, 107, 165, 36, 114, 85, 219, 180, 207, 181, 163, 5, 249, 182, 57, 146, 108, 7, 68, 112, 113, 14, 73, 81, 168, 109, 128, 115, 104, 192, 14, 84, 53, 157, 136, 249, 191, 141, 21, 93, 216, 80, 30, 3, 249, 48, 202, 97, 118, 154, 143, 108, 67, 132, 96, 184, 9, 62, 204, 245, 3, 110, 10, 64, 246, 63, 223, 111, 173, 100, 218, 28, 18, 181, 250, 144, 196, 153, 12, 11, 194, 87, 111, 204, 73, 79, 99, 178, 178, 208, 196, 243, 209, 54, 73, 161, 86, 239, 201, 191, 100, 131, 232, 146, 39 }, 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "PasswordHash", "PasswordSalt", "RoleId" },
                values: new object[] { 2, "Roger", new byte[] { 186, 122, 249, 45, 81, 194, 96, 245, 196, 146, 116, 254, 122, 32, 131, 81, 232, 75, 151, 58, 151, 143, 219, 105, 101, 29, 101, 5, 122, 123, 228, 13, 77, 39, 228, 104, 91, 229, 247, 80, 254, 249, 182, 68, 0, 47, 54, 111, 19, 245, 230, 131, 146, 20, 164, 151, 217, 121, 227, 185, 254, 198, 142, 65 }, new byte[] { 240, 170, 87, 15, 126, 15, 202, 230, 128, 193, 58, 234, 146, 132, 107, 142, 145, 154, 155, 21, 7, 176, 152, 110, 107, 165, 36, 114, 85, 219, 180, 207, 181, 163, 5, 249, 182, 57, 146, 108, 7, 68, 112, 113, 14, 73, 81, 168, 109, 128, 115, 104, 192, 14, 84, 53, 157, 136, 249, 191, 141, 21, 93, 216, 80, 30, 3, 249, 48, 202, 97, 118, 154, 143, 108, 67, 132, 96, 184, 9, 62, 204, 245, 3, 110, 10, 64, 246, 63, 223, 111, 173, 100, 218, 28, 18, 181, 250, 144, 196, 153, 12, 11, 194, 87, 111, 204, 73, 79, 99, 178, 178, 208, 196, 243, 209, 54, 73, 161, 86, 239, 201, 191, 100, 131, 232, 146, 39 }, 2 });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "Id", "Agility", "Defeats", "Fights", "Health", "Intelligence", "ModificationDateTime", "Name", "Strength", "UserId", "Victories", "charType" },
                values: new object[] { 1, 0, 0, 0, 100, 20, null, "Luke", 10, 1, 0, 0 });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "Id", "Agility", "Defeats", "Fights", "Health", "Intelligence", "ModificationDateTime", "Name", "Strength", "UserId", "Victories", "charType" },
                values: new object[] { 2, 0, 0, 0, 100, 50, null, "Gendalf", 20, 1, 0, 0 });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "Id", "Agility", "Defeats", "Fights", "Health", "Intelligence", "ModificationDateTime", "Name", "Strength", "UserId", "Victories", "charType" },
                values: new object[] { 3, 0, 0, 0, 100, 10, null, "Dart Vader", 30, 2, 0, 0 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 1, 3 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 2, 3 });

            migrationBuilder.InsertData(
                table: "CharacterSkills",
                columns: new[] { "CharacterId", "SkillId" },
                values: new object[] { 3, 1 });

            migrationBuilder.InsertData(
                table: "Weapons",
                columns: new[] { "Id", "CharacterId", "Damage", "Name" },
                values: new object[] { 1, 1, 10, "FireStick" });

            migrationBuilder.InsertData(
                table: "Weapons",
                columns: new[] { "Id", "CharacterId", "Damage", "Name" },
                values: new object[] { 2, 2, 20, "Axe" });

            migrationBuilder.InsertData(
                table: "Weapons",
                columns: new[] { "Id", "CharacterId", "Damage", "Name" },
                values: new object[] { 3, 3, 20, "Sward" });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkills_SkillId",
                table: "CharacterSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_CharacterId",
                table: "Weapons",
                column: "CharacterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterSkills");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
