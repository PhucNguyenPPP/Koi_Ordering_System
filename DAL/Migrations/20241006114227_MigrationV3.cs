using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class MigrationV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    AirportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirportName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.AirportId);
                });

            migrationBuilder.CreateTable(
                name: "Breeds",
                columns: table => new
                {
                    BreedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breeds", x => x.BreedId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartureAirportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArrivalAirportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_Flights_Airports_ArrivalAirportId",
                        column: x => x.ArrivalAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId");
                    table.ForeignKey(
                        name: "FK_Flights_Airports_DepartureAirportId",
                        column: x => x.DepartureAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId");
                });

            migrationBuilder.CreateTable(
                name: "StorageProvinces",
                columns: table => new
                {
                    StorageProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    AirportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageProvinces", x => x.StorageProvinceId);
                    table.ForeignKey(
                        name: "FK_StorageProvinces_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFees",
                columns: table => new
                {
                    ShippingFeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageProvinceJpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageProvinceVnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFees", x => x.ShippingFeeId);
                    table.ForeignKey(
                        name: "FK_ShippingFees_StorageProvinces_StorageProvinceJpId",
                        column: x => x.StorageProvinceJpId,
                        principalTable: "StorageProvinces",
                        principalColumn: "StorageProvinceId");
                    table.ForeignKey(
                        name: "FK_ShippingFees_StorageProvinces_StorageProvinceVnId",
                        column: x => x.StorageProvinceVnId,
                        principalTable: "StorageProvinces",
                        principalColumn: "StorageProvinceId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarLink = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OtpCode = table.Column<int>(type: "int", nullable: true),
                    OtpExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_StorageProvinces_StorageProvinceId",
                        column: x => x.StorageProvinceId,
                        principalTable: "StorageProvinces",
                        principalColumn: "StorageProvinceId");
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "KoiFarms",
                columns: table => new
                {
                    KoiFarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FarmDescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FarmAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FarmAvatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KoiFarmManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiFarms", x => x.KoiFarmId);
                    table.ForeignKey(
                        name: "FK_KoiFarms_StorageProvinces_StorageProvinceId",
                        column: x => x.StorageProvinceId,
                        principalTable: "StorageProvinces",
                        principalColumn: "StorageProvinceId");
                    table.ForeignKey(
                        name: "FK_KoiFarms_Users_KoiFarmManagerId",
                        column: x => x.KoiFarmManagerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PercentageRefund = table.Column<int>(type: "int", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_Policies_KoiFarms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "KoiFarms",
                        principalColumn: "KoiFarmId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippingFee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlightId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId");
                    table.ForeignKey(
                        name: "FK_Orders_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Kois",
                columns: table => new
                {
                    KoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarLink = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificationLink = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kois", x => x.KoiId);
                    table.ForeignKey(
                        name: "FK_Kois_KoiFarms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "KoiFarms",
                        principalColumn: "KoiFarmId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kois_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "OrderStorages",
                columns: table => new
                {
                    OrderStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    StorageProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipperId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStorages", x => x.OrderStorageId);
                    table.ForeignKey(
                        name: "FK_OrderStorages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderStorages_StorageProvinces_StorageProvinceId",
                        column: x => x.StorageProvinceId,
                        principalTable: "StorageProvinces",
                        principalColumn: "StorageProvinceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderStorages_Users_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "RefundRequests",
                columns: table => new
                {
                    RefundRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequests", x => x.RefundRequestId);
                    table.ForeignKey(
                        name: "FK_RefundRequests_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TransactionInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodTransaction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId");
                    table.ForeignKey(
                        name: "FK_CartItems_Kois_KoiId",
                        column: x => x.KoiId,
                        principalTable: "Kois",
                        principalColumn: "KoiId");
                });

            migrationBuilder.CreateTable(
                name: "KoiBreeds",
                columns: table => new
                {
                    KoiBreedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BreedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiBreeds", x => x.KoiBreedId);
                    table.ForeignKey(
                        name: "FK_KoiBreeds_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "BreedId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KoiBreeds_Kois_KoiId",
                        column: x => x.KoiId,
                        principalTable: "Kois",
                        principalColumn: "KoiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KoiImages",
                columns: table => new
                {
                    KoiImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiImages", x => x.KoiImageId);
                    table.ForeignKey(
                        name: "FK_KoiImages_Kois_KoiId",
                        column: x => x.KoiId,
                        principalTable: "Kois",
                        principalColumn: "KoiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundRequestMedia",
                columns: table => new
                {
                    RefundRequestMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequestMedia", x => x.RefundRequestMediaId);
                    table.ForeignKey(
                        name: "FK_RefundRequestMedia_RefundRequests_RefundRequestId",
                        column: x => x.RefundRequestId,
                        principalTable: "RefundRequests",
                        principalColumn: "RefundRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "AirportId", "Address", "AirportName" },
                values: new object[,]
                {
                    { new Guid("626db7e0-1198-49f1-af5c-fa8d24f1ce19"), "!233123132", "Los Angeles International Airport" },
                    { new Guid("9c26a8b1-ddf5-4f74-8088-d1ff471a0526"), "12333", "Tokyo International Airport" }
                });

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "BreedId", "Name" },
                values: new object[,]
                {
                    { new Guid("c4eb5367-8656-4f29-b67f-c45763c8e987"), "Sanke" },
                    { new Guid("fbd7ed77-3298-4316-ad38-c37a7978c3ed"), "Kohaku" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("07d78700-442e-4a96-8335-4eb17b7b919b"), "Admin" },
                    { new Guid("9cf7e90a-d8c1-482c-a44e-5df7450f7290"), "User" }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "FlightId", "ArrivalAirportId", "Code", "DepartureAirportId", "Status" },
                values: new object[] { new Guid("50589e90-8f54-48e9-80b3-0c4e2da5c539"), new Guid("626db7e0-1198-49f1-af5c-fa8d24f1ce19"), "FL123", new Guid("9c26a8b1-ddf5-4f74-8088-d1ff471a0526"), "On Time" });

            migrationBuilder.InsertData(
                table: "StorageProvinces",
                columns: new[] { "StorageProvinceId", "Address", "AirportId", "Country", "ProvinceName", "Status", "StorageName" },
                values: new object[] { new Guid("b4822413-a428-4c09-ae55-b2920892ce6e"), "Pending", new Guid("9c26a8b1-ddf5-4f74-8088-d1ff471a0526"), "Vietnam", "Hanoi", false, "Hanoi Storage" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "AvatarLink", "DateOfBirth", "Email", "FullName", "Gender", "OtpCode", "OtpExpiredTime", "PasswordHash", "Phone", "RoleId", "Salt", "Status", "StorageProvinceId", "UserName" },
                values: new object[,]
                {
                    { new Guid("0a1226c4-4c88-439b-a762-b18b3f816790"), "456 Main St, City, Country", "https://example.com/avatar/jane_doe.jpg", new DateTime(1992, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "janedoe@example.com", "Jane Doe", "Female", null, null, new byte[] { 112, 97, 115, 115, 119, 111, 114, 100, 72, 97, 115, 104, 86, 97, 108, 117, 101 }, "0987654321", new Guid("9cf7e90a-d8c1-482c-a44e-5df7450f7290"), new byte[] { 115, 97, 108, 116, 86, 97, 108, 117, 101 }, true, null, "jane_doe" },
                    { new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c"), "123 Main St, City, Country", "https://example.com/avatar/john_doe.jpg", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "johndoe@example.com", "John Doe", "Male", null, null, new byte[] { 112, 97, 115, 115, 119, 111, 114, 100, 72, 97, 115, 104, 86, 97, 108, 117, 101 }, "1234567890", new Guid("07d78700-442e-4a96-8335-4eb17b7b919b"), new byte[] { 115, 97, 108, 116, 86, 97, 108, 117, 101 }, true, null, "john_doe" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "CartId", "CreatedDate", "UserId" },
                values: new object[] { new Guid("20c2c145-a01b-4bc7-b817-8946b94f2037"), new DateTime(2024, 10, 6, 18, 42, 27, 305, DateTimeKind.Local).AddTicks(7010), new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c") });

            migrationBuilder.InsertData(
                table: "KoiFarms",
                columns: new[] { "KoiFarmId", "FarmAddress", "FarmAvatar", "FarmDescription", "FarmName", "KoiFarmManagerId", "StorageProvinceId" },
                values: new object[] { new Guid("ede8cb4f-ec5f-4de2-a1f2-fec6f1596d9e"), "123 Farm Rd", "farm_avatar_link", "High-quality Koi farm", "Koi Farm 1", new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c"), null });

            migrationBuilder.InsertData(
                table: "RefreshTokens",
                columns: new[] { "RefreshTokenId", "ExpiredAt", "IsValid", "JwtId", "RefreshToken1", "UserId" },
                values: new object[] { new Guid("a058bf84-a6ba-4fd5-9e0e-64d1fcf9fd8b"), new DateTime(2024, 11, 5, 18, 42, 27, 302, DateTimeKind.Local).AddTicks(9813), false, "abcd1234", "abcd1234", new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c") });

            migrationBuilder.InsertData(
                table: "Kois",
                columns: new[] { "KoiId", "AvatarLink", "CertificationLink", "Description", "Dob", "FarmId", "Gender", "Name", "OrderId", "Price", "Status" },
                values: new object[] { new Guid("643bd313-2e49-4a14-8ae5-a252136e0f5c"), "avatar_link_1", "certification_link_1", null, new DateTime(2023, 10, 6, 18, 42, 27, 302, DateTimeKind.Local).AddTicks(6571), new Guid("ede8cb4f-ec5f-4de2-a1f2-fec6f1596d9e"), "Male", "Koi A", null, 5000m, false });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "PolicyId", "Description", "FarmId", "PercentageRefund", "PolicyName" },
                values: new object[] { new Guid("f9af0623-78bd-4775-a0fa-1b827b5167a1"), "Refund Policy", new Guid("ede8cb4f-ec5f-4de2-a1f2-fec6f1596d9e"), 0, "Refund Policy" });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "CartItemId", "CartId", "KoiId", "Quantity", "UnitPrice" },
                values: new object[] { new Guid("e1c9089f-74da-4123-9732-15eb81d78f49"), new Guid("20c2c145-a01b-4bc7-b817-8946b94f2037"), new Guid("643bd313-2e49-4a14-8ae5-a252136e0f5c"), 1, 5000m });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "Address", "CreatedDate", "CustomerId", "Feedback", "FlightId", "Height", "Length", "OrderNumber", "Phone", "PolicyId", "Rating", "ShippingFee", "Status", "TotalPrice", "Weight", "Width" },
                values: new object[] { new Guid("bf2fb33a-0500-4ef6-bcc6-abe4b819fa4d"), "123 Main St", new DateTime(2024, 10, 6, 18, 42, 27, 302, DateTimeKind.Local).AddTicks(8851), new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c"), null, null, null, null, "ORD123", "1234567890", new Guid("f9af0623-78bd-4775-a0fa-1b827b5167a1"), null, "1111", "Processing", 10000m, null, null });

            migrationBuilder.InsertData(
                table: "OrderStorages",
                columns: new[] { "OrderStorageId", "ArrivalTime", "OrderId", "ShipperId", "Status", "StorageProvinceId" },
                values: new object[] { new Guid("3cb4e0fc-c0ac-4c19-afcf-4ecc5e2a384a"), new DateTime(2024, 10, 6, 18, 42, 27, 302, DateTimeKind.Local).AddTicks(9192), new Guid("bf2fb33a-0500-4ef6-bcc6-abe4b819fa4d"), new Guid("b886a834-419a-4a44-8823-a9c4d96c3b2c"), false, new Guid("b4822413-a428-4c09-ae55-b2920892ce6e") });

            migrationBuilder.InsertData(
                table: "RefundRequests",
                columns: new[] { "RefundRequestId", "CreatedDate", "Description", "OrderId", "Response", "Status" },
                values: new object[] { new Guid("caaa8099-de72-4d02-b079-116cae4b49e8"), new DateTime(2024, 10, 6, 18, 42, 27, 303, DateTimeKind.Local).AddTicks(211), "Pending", new Guid("bf2fb33a-0500-4ef6-bcc6-abe4b819fa4d"), "Pending", "Pending" });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "TransactionId", "CreatedDate", "OrderId", "PaymentMethodTransaction", "Status", "TransactionInfo", "TransactionNumber" },
                values: new object[] { new Guid("d6f59da0-b09d-4c70-9fe5-ddc4b29fe867"), new DateTime(2024, 10, 6, 18, 42, 27, 305, DateTimeKind.Local).AddTicks(4980), new Guid("bf2fb33a-0500-4ef6-bcc6-abe4b819fa4d"), "Credit Card", "Success", "111", "!11" });

            migrationBuilder.InsertData(
                table: "RefundRequestMedia",
                columns: new[] { "RefundRequestMediaId", "Link", "RefundRequestId" },
                values: new object[] { new Guid("e124dcf5-5098-46b9-83b3-751236dec2d5"), "Pending", new Guid("caaa8099-de72-4d02-b079-116cae4b49e8") });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_KoiId",
                table: "CartItems",
                column: "KoiId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalAirportId",
                table: "Flights",
                column: "ArrivalAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureAirportId",
                table: "Flights",
                column: "DepartureAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiBreeds_BreedId",
                table: "KoiBreeds",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiBreeds_KoiId",
                table: "KoiBreeds",
                column: "KoiId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFarms_KoiFarmManagerId",
                table: "KoiFarms",
                column: "KoiFarmManagerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KoiFarms_StorageProvinceId",
                table: "KoiFarms",
                column: "StorageProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiImages_KoiId",
                table: "KoiImages",
                column: "KoiId");

            migrationBuilder.CreateIndex(
                name: "IX_Kois_FarmId",
                table: "Kois",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Kois_OrderId",
                table: "Kois",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FlightId",
                table: "Orders",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PolicyId",
                table: "Orders",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStorages_OrderId",
                table: "OrderStorages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStorages_ShipperId",
                table: "OrderStorages",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStorages_StorageProvinceId",
                table: "OrderStorages",
                column: "StorageProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_FarmId",
                table: "Policies",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequestMedia_RefundRequestId",
                table: "RefundRequestMedia",
                column: "RefundRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_OrderId",
                table: "RefundRequests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFees_StorageProvinceJpId",
                table: "ShippingFees",
                column: "StorageProvinceJpId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFees_StorageProvinceVnId",
                table: "ShippingFees",
                column: "StorageProvinceVnId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProvinces_AirportId",
                table: "StorageProvinces",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StorageProvinceId",
                table: "Users",
                column: "StorageProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "KoiBreeds");

            migrationBuilder.DropTable(
                name: "KoiImages");

            migrationBuilder.DropTable(
                name: "OrderStorages");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RefundRequestMedia");

            migrationBuilder.DropTable(
                name: "ShippingFees");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Breeds");

            migrationBuilder.DropTable(
                name: "Kois");

            migrationBuilder.DropTable(
                name: "RefundRequests");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "KoiFarms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "StorageProvinces");

            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
