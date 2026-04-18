using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServicePago : IServicePago
    {

        private readonly IRepositoryPago _repositoryPago;
        private readonly IRepositorySubasta _repositorySubasta;
        private readonly IMapper _mapper;

        public ServicePago(IRepositoryPago repositoryPago, IRepositorySubasta repositorySubasta, IMapper mapper)
        {
            _repositoryPago = repositoryPago;
            _repositorySubasta = repositorySubasta;
            _mapper = mapper;
        }

        public async Task ConfirmarPagoAsync(int idPago)
        {
            await _repositoryPago.ConfirmarPagoAsync(idPago);
        }
        public async Task<PagoDTO?> GetBySubastaAsync(int idSubasta)
        {
            var entity = await _repositoryPago.GetBySubastaAsync(idSubasta);
            if (entity == null)
                throw new Exception("No se encontró el pago para la subasta especificada.");
            return _mapper.Map<PagoDTO>(entity);
        }

        public async Task<int> RegistrarPagoAsync(int idSubasta)
        {
            //Busca la subasta por Id 
            var subasta = await _repositorySubasta.FindByIdAsync(idSubasta);
            if (subasta == null)
                throw new Exception("Subasta no encontrada.");

            //para subastas que están finalizadas
            if (subasta.IdEstadoSubasta != 2)
                throw new InvalidOperationException(
                    "Solo se puede registrar el pago de una subasta finalizada.");

            if (subasta.ResultadoSubasta == null)
                throw new InvalidOperationException(
                    "Esta subasta no tiene ganador, no se puede registrar el pago.");

            //Si ya existe un pago registrado para esta subasta, no se puede registrar otro
            bool existePago = await _repositoryPago.ExistePagoParaSubastaAsync(idSubasta);
            if (existePago)
                throw new InvalidOperationException(
                    "Esta subasta ya tiene un pago registrado.");

            var pago = new Pago
            {
                IdSubasta = idSubasta,
                Monto = subasta.ResultadoSubasta.MontoFinal,
                IdEstadoPago = 1,
                FechaRegistro = DateTime.Now
            };
            return await _repositoryPago.AddAsync(pago);
        }
    }
}
