﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tadbir.Entities;
using tadbir.Repository.Interfaces;
using tadbir.Service.DTOs.InvoiceDTOs;
using tadbir.Service.Interfaces;
using System.Linq;
using System.Threading;

namespace tadbir.Service.Implementations
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _invoiceRepository = _unitOfWork.InvoiceRepository;
        }

        public async Task<InvoiceListDto> AddNewInvoiceAsync(InvoiceDto invoiceDto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Invoice>(invoiceDto);
            _invoiceRepository.Add(entity);

            entity.Rows = new List<InvoiceRow>();
            foreach (var item in invoiceDto.Rows)
            {
                entity.Rows.Add(_mapper.Map<InvoiceRow>(item));
            }
            await _unitOfWork.CommitAsync();
            entity = await _invoiceRepository.GetWithRowsAsync(entity.Id);
            return _mapper.Map<InvoiceListDto>(entity);
        }

        public async Task DeleteInvoiceAsync(long id, CancellationToken cancellationToken)
        {
            await _invoiceRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<InvoiceListDto> EditInvoiceAsync(InvoiceDto invoiceDto, long invoiceId, CancellationToken cancellationToken)
        {
            var entity = await _invoiceRepository.GetWithRowsAsync(invoiceId);
            if (entity == null)
                throw new KeyNotFoundException();
            _mapper.Map(invoiceDto, entity);

            await _invoiceRepository.UpdateAsync(entity, invoiceId);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<InvoiceListDto>(entity);
        }

        public async Task<DetailedInvoiceDto> GetInvoiceAsync(long id, CancellationToken cancellationToken)
        {
            var entity = await _invoiceRepository.GetWithRowsAsync(id);
            return _mapper.Map<DetailedInvoiceDto>(entity);
        }

        public async Task<IEnumerable<InvoiceListDto>> GetInvoiceListAsync(CancellationToken cancellationToken)
        {
            var entities = await _invoiceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InvoiceListDto>>(entities);
        }
    }
}
