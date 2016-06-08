﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface ISelectionStrategy
    {
        Task<ICollection<long>> GenerateQuery(IEnumerable<object[]> labelled, IEnumerable<object[]> unlabelled, IDataFormat format, int batchSize);
    }
}
