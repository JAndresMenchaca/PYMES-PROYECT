using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_Pymes.Models.DB;

public partial class DbPymesContext : DbContext
{
    public DbPymesContext()
    {
    }

    public DbPymesContext(DbContextOptions<DbPymesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BusinessManager> BusinessManagers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Enterprise> Enterprises { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Manufacturing> Manufacturings { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Producer> Producers { get; set; }

    public virtual DbSet<ProducerCompany> ProducerCompanies { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductionDetail> ProductionDetails { get; set; }

    public virtual DbSet<RawMaterial> RawMaterials { get; set; }

    public virtual DbSet<Sector> Sectors { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    public virtual DbSet<TownShip> TownShips { get; set; }

    public virtual DbSet<UnitType> UnitTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
     //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
       // => optionsBuilder.UseSqlServer("Server=DESKTOP-ADJ9ORU\\SQLEXPRESS; database=dbPymes; integrated security=true; Encrypt=False;");
    }
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessManager>(entity =>
        {
            entity.ToTable("BusinessManager");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CorporateNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("corporateNumber");
            entity.Property(e => e.IdEnterprise).HasColumnName("idEnterprise");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.BusinessManager)
                .HasForeignKey<BusinessManager>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BusinessManager_Person");

            entity.HasOne(d => d.IdEnterpriseNavigation).WithMany(p => p.BusinessManagers)
                .HasForeignKey(d => d.IdEnterprise)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BusinessManager_Enterprise");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<Enterprise>(entity =>
        {
            entity.ToTable("Enterprise");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.GroupName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("groupName");
            entity.Property(e => e.IdTownShip).HasColumnName("idTownShip");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdTownShipNavigation).WithMany(p => p.Enterprises)
                .HasForeignKey(d => d.IdTownShip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enterprise_TownShip");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FilePath).HasColumnName("filePath");
            entity.Property(e => e.IdProduct).HasColumnName("idProduct");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(55)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_Product");
        });

        modelBuilder.Entity<Manufacturing>(entity =>
        {
            entity.ToTable("Manufacturing");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostProduction)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("costProduction");
            entity.Property(e => e.IdProduct).HasColumnName("idProduct");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Manufacturings)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manufacturing_Product");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("Person");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ci)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ci");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.SecondLastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("secondLastName");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Producer>(entity =>
        {
            entity.ToTable("Producer");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Latitude)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("longitude");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Producer)
                .HasForeignKey<Producer>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producer_Person");
        });

        modelBuilder.Entity<ProducerCompany>(entity =>
        {
            entity.HasKey(e => new { e.IdProducer, e.IdEnterprise });

            entity.ToTable("producerCompany");

            entity.Property(e => e.IdProducer).HasColumnName("idProducer");
            entity.Property(e => e.IdEnterprise).HasColumnName("idEnterprise");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");

            entity.HasOne(d => d.IdEnterpriseNavigation).WithMany(p => p.ProducerCompanies)
                .HasForeignKey(d => d.IdEnterprise)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producerCompany_Enterprise");

            entity.HasOne(d => d.IdProducerNavigation).WithMany(p => p.ProducerCompanies)
                .HasForeignKey(d => d.IdProducer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producerCompany_Producer");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrise)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("basePrise");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IdCategory).HasColumnName("idCategory");
            entity.Property(e => e.IdProducer).HasColumnName("idProducer");
            entity.Property(e => e.IdSector).HasColumnName("idSector");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.ManufacturingNeed)
                .HasComment("1 si\r\n2 no")
                .HasColumnName("manufacturingNeed");
            entity.Property(e => e.Name)
                .HasMaxLength(55)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UnitMeasure)
                .HasMaxLength(55)
                .IsUnicode(false)
                .HasColumnName("unitMeasure");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.IdProducerNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdProducer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Producer");

            entity.HasOne(d => d.IdSectorNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdSector)
                .HasConstraintName("FK_Product_Sector");
        });

        modelBuilder.Entity<ProductionDetail>(entity =>
        {
            entity.HasKey(e => new { e.IdManufacturing, e.IdRawMaterial });

            entity.ToTable("ProductionDetail");

            entity.Property(e => e.IdManufacturing).HasColumnName("idManufacturing");
            entity.Property(e => e.IdRawMaterial).HasColumnName("idRawMaterial");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdManufacturingNavigation).WithMany(p => p.ProductionDetails)
                .HasForeignKey(d => d.IdManufacturing)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionDetail_Manufacturing");

            entity.HasOne(d => d.IdRawMaterialNavigation).WithMany(p => p.ProductionDetails)
                .HasForeignKey(d => d.IdRawMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionDetail_RawMaterial");
        });

        modelBuilder.Entity<RawMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_rawmaterial");

            entity.ToTable("RawMaterial");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IdUnitType).HasColumnName("idUnitType");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(55)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("unitPrice");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdUnitTypeNavigation).WithMany(p => p.RawMaterials)
                .HasForeignKey(d => d.IdUnitType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RawMaterial_UnitType");
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.ToTable("Sector");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CapacityMax).HasColumnName("capacityMax");
            entity.Property(e => e.IdWareHouse).HasColumnName("idWareHouse");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdWareHouseNavigation).WithMany(p => p.Sectors)
                .HasForeignKey(d => d.IdWareHouse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sector_Warehouse");
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.ToTable("Specification");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataType)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("dataType");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IdProduct).HasColumnName("idProduct");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Value)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("value");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Specifications)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specification_Product");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdProducer).HasColumnName("idProducer");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.SecondLastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("secondLastName");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdProducerNavigation).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.IdProducer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supplier_Producer");
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.ToTable("Supply");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdRawMaterial).HasColumnName("idRawMaterial");
            entity.Property(e => e.IdSupplier).HasColumnName("idSupplier");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("unitPrice");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("userID");

            entity.HasOne(d => d.IdRawMaterialNavigation).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.IdRawMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supply_RawMaterial");

            entity.HasOne(d => d.IdSupplierNavigation).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.IdSupplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supply_Supplier");
        });

        modelBuilder.Entity<Town>(entity =>
        {
            entity.ToTable("Town");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TownShip>(entity =>
        {
            entity.ToTable("TownShip");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdTown).HasColumnName("idTown");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.IdTownNavigation).WithMany(p => p.TownShips)
                .HasForeignKey(d => d.IdTown)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TownShip_Town");
        });

        modelBuilder.Entity<UnitType>(entity =>
        {
            entity.ToTable("UnitType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FirstLogin)
                .HasDefaultValueSql("((1))")
                .HasColumnName("firstLogin");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Password)
                .HasMaxLength(35)
                .HasColumnName("password");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("administratorUnivalle\r\nBusinessManager\r\nProducer\r\n")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.UserName)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("userName");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Person");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouse");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CapacityMax).HasColumnName("capacityMax");
            entity.Property(e => e.LastUpdate)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdate");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
