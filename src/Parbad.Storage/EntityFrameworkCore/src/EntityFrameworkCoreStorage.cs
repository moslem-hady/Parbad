﻿// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Parbad.Storage.Abstractions;
using Parbad.Storage.Abstractions.Models;
using Parbad.Storage.EntityFrameworkCore.Context;
using Parbad.Storage.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parbad.Storage.EntityFrameworkCore
{
    /// <summary>
    /// EntityFramework Core implementation of <see cref="IStorage"/>.
    /// </summary>
    public class EntityFrameworkCoreStorage : IStorage
    {
        /// <summary>
        /// Initializes an instance of <see cref="EntityFrameworkCoreStorage"/>.
        /// </summary>
        /// <param name="context"></param>
        public EntityFrameworkCoreStorage(ParbadDataContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        public virtual IQueryable<Payment> Payments => Context.Payments.Select(Mapper.ToPaymentModel());

        /// <inheritdoc />
        public virtual IQueryable<Transaction> Transactions => Context.Transactions.Select(Mapper.ToTransactionModel());

        /// <summary>
        /// Parbad DbContext.
        /// </summary>
        protected ParbadDataContext Context { get; }

        /// <inheritdoc />
        public virtual async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            var entity = payment.ToPaymentEntity();
            entity.CreatedOn = DateTime.UtcNow;

            Context.Payments.Add(entity);

            await Context.SaveChangesAsync(cancellationToken);

            Context.Entry(entity).State = EntityState.Detached;

            payment.Id = entity.Id;
        }

        /// <inheritdoc />
        public virtual async Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            var record = await Context
                .Payments
                .AsNoTracking()
                .SingleOrDefaultAsync(model => model.Id == payment.Id, cancellationToken);

            if (record == null) throw new InvalidOperationException($"No payment records found in database with id {payment.Id}");

            Mapper.ToPaymentEntity(payment, record);
            record.UpdatedOn = DateTime.UtcNow;

            Context.Payments.Update(record);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeletePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            var record = await Context
                .Payments
                .AsNoTracking()
                .SingleOrDefaultAsync(model => model.Id == payment.Id, cancellationToken);

            if (record == null) throw new InvalidOperationException($"No payment records found in database with id {payment.Id}");

            Context.Payments.Remove(record);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var entity = transaction.ToTransactionEntity();
            entity.CreatedOn = DateTime.UtcNow;

            Context.Transactions.Add(entity);

            await Context.SaveChangesAsync(cancellationToken);

            transaction.Id = entity.Id;
        }

        /// <inheritdoc />
        public virtual async Task UpdateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var record = await Context
                .Transactions
                .SingleOrDefaultAsync(model => model.Id == transaction.Id, cancellationToken);

            if (record == null) throw new InvalidOperationException($"No transaction records found in database with id {transaction.Id}");

            Mapper.ToTransactionEntity(transaction, record);
            record.UpdatedOn = DateTime.UtcNow;

            Context.Transactions.Update(record);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var record = await Context
                .Transactions
                .SingleOrDefaultAsync(model => model.Id == transaction.Id, cancellationToken);

            if (record == null) throw new InvalidOperationException($"No transaction records found in database with id {transaction.Id}");

            Context.Transactions.Remove(record);

            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
