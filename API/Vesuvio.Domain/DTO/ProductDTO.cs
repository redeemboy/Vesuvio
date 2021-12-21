using Vesuvio.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Vesuvio.Domain.DTO
{
    public class CommonProductModel
    {
        public Guid? Id { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public string FinishedProductSize { get; set; }
        [Required]
        public string FinishedProductWeight { get; set; }
        [Required]
        public string AvailableMaterial { get; set; }
        [Required]
        public string AvailableColor { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string LeadTime { get; set; }
        [Required]
        public string Tag { get; set; }
        public string Status { get; set; }
    }

    public class ProductDTO : CommonProductModel
    {
        public bool RemoveImage { get; set; }

        public Product MapToProduct(ProductDTO dto, Product product, Guid userId)
        {
            product.Id = dto.Id.HasValue ? (Guid)dto.Id : product.Id;
            product.SKU = dto.SKU;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.FinishedProductSize = dto.FinishedProductSize;
            product.FinishedProductWeight = dto.FinishedProductWeight;
            product.AvailableMaterial = dto.AvailableMaterial;
            product.AvailableColor = dto.AvailableColor;
            product.Price = dto.Price;
            product.LeadTime = dto.LeadTime;
            product.Tag = dto.Tag;
            product.ImageUrl = dto.ImageUrl;

            if (!dto.Id.HasValue)
            {
                product.CreatedBy = userId;
                product.CreatedDate = DateTime.Now;
                product.Status = "INSTOCK";
            }
            else
            {
                product.ModifiedDate = DateTime.Now;
                product.ModifiedBy = userId;
                product.Status = dto.Status;
            }

            return product;
        }
    }

    public class ProductResponseDTO : CommonProductModel
    {
        public ModelUser CreatedUser { get; set; }
        public ModelUser ModifiedUser { get; set; }

        public ProductResponseDTO MapToProductDTO(Product product)
        {
            ProductResponseDTO dto = new ProductResponseDTO();

            dto.Id = product.Id;
            dto.SKU = product.SKU;
            dto.Name = product.Name;
            dto.Description = product.Description;
            dto.FinishedProductSize = product.FinishedProductSize;
            dto.FinishedProductWeight = product.FinishedProductWeight;
            dto.AvailableMaterial = product.AvailableMaterial;
            dto.AvailableColor = product.AvailableColor;
            dto.Price = product.Price;
            dto.LeadTime = product.LeadTime;
            dto.Tag = product.Tag;
            dto.Status = product.Status;
            dto.ImageUrl = product.ImageUrl;


            dto.CreatedUser = new ModelUser { UserId = product.CreatedBy, UserName = product.CreatedByUser.UserName };

            if (product.ModifiedByUser != null)
            {
                dto.ModifiedUser = new ModelUser { UserId = product.ModifiedBy, UserName = product.ModifiedByUser.UserName };
            }

            return dto;
        }
    }
}