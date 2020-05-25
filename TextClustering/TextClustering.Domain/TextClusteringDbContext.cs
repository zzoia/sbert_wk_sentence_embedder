using Microsoft.EntityFrameworkCore;
using TextClustering.Domain.Entities;

namespace TextClustering.Domain
{
    public class TextClusteringDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Cluster> Clusters { get; set; }

        public DbSet<Dataset> Datasets { get; set; }

        public DbSet<DatasetClustering> DatasetClusterings { get; set; }

        public DbSet<DatasetText> DatasetTexts { get; set; }

        public DbSet<ClusterDatasetText> ClusterDatasetTexts { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<TopicToken> TopicTokens { get; set; }

        public TextClusteringDbContext(DbContextOptions<TextClusteringDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(user => user.FirstName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(user => user.LastName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(user => user.Password)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .HasMany(user => user.DatasetClusterings)
                .WithOne(clustering => clustering.CreatedByUser)
                .HasForeignKey(clustering => clustering.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<User>()
                .HasMany(user => user.Datasets)
                .WithOne(set => set.CreatedByUser)
                .HasForeignKey(set => set.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Cluster>()
                .HasMany(cluster => cluster.ClusterDatasetTexts)
                .WithOne(text => text.Cluster)
                .HasForeignKey(text => text.ClusterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Cluster>()
                .HasMany(cluster => cluster.Topics)
                .WithOne(topic => topic.Cluster)
                .HasForeignKey(topic => topic.ClusterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Cluster>()
                .HasOne(cluster => cluster.DatasetClustering)
                .WithMany(clustering => clustering.Clusters)
                .HasForeignKey(cluster => cluster.DatasetClusteringId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Dataset>()
                .Property(set => set.Name)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<Dataset>()
                .Property(set => set.Description)
                .IsRequired()
                .HasMaxLength(1024);

            modelBuilder
                .Entity<Dataset>()
                .HasMany(set => set.Texts)
                .WithOne(text => text.Dataset)
                .HasForeignKey(text => text.DatasetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Dataset>()
                .HasMany(set => set.DatasetClusterings)
                .WithOne(clustering => clustering.Dataset)
                .HasForeignKey(clustering => clustering.DatasetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Dataset>()
                .Property(set => set.Name)
                .HasMaxLength(256);

            modelBuilder
                .Entity<Dataset>()
                .Property(set => set.Description)
                .HasMaxLength(1024);

            modelBuilder
                .Entity<DatasetText>()
                .HasIndex(text => new {text.Key, text.DatasetId})
                .IsUnique();

            modelBuilder
                .Entity<DatasetText>()
                .HasMany(text => text.ClusterDatasetTexts)
                .WithOne(text => text.DatasetText)
                .HasForeignKey(text => text.DatasetTextId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<DatasetText>()
                .Property(text => text.Key)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<DatasetClustering>()
                .Property(clustering => clustering.Description)
                .HasMaxLength(1024);
            
            modelBuilder
                .Entity<DatasetClustering>()
                .Property(clustering => clustering.Description)
                .HasMaxLength(1024);

            modelBuilder
                .Entity<Topic>()
                .HasMany(topic => topic.Tokens)
                .WithOne(token => token.Topic)
                .HasForeignKey(token => token.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ClusterDatasetText>()
                .HasKey(text => new { text.ClusterId, text.DatasetTextId });
        }
    }
}