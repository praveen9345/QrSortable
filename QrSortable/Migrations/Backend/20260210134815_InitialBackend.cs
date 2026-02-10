using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrSortable.Migrations.Backend
{
    /// <inheritdoc />
    public partial class InitialBackend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderedDto",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsUpdateData = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CodeType = table.Column<string>(type: "TEXT", nullable: false),
                    PageType = table.Column<string>(type: "TEXT", nullable: false),
                    ProductQuantity = table.Column<string>(type: "TEXT", nullable: false),
                    DateTime = table.Column<string>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Street = table.Column<string>(type: "TEXT", nullable: false),
                    HouseNo = table.Column<string>(type: "TEXT", nullable: false),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: false),
                    ShipmentTracking = table.Column<string>(type: "TEXT", nullable: false),
                    StatusOfOrder = table.Column<string>(type: "TEXT", nullable: false),
                    PdfFiles = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedDto", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StorageEntriesDto",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsUpdateData = table.Column<string>(type: "TEXT", nullable: false),
                    StorageId = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    BarcodeValue = table.Column<string>(type: "TEXT", nullable: false),
                    BarcodeType = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    SearchInfo = table.Column<string>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImageList = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundColorHex = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageEntriesDto", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderedDto");

            migrationBuilder.DropTable(
                name: "StorageEntriesDto");
        }
    }
}
