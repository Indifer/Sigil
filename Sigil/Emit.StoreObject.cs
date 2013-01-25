﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void StoreObject<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            StoreObject(typeof(ValueType), isVolatile, unaligned);
        }

        public void StoreObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4", "unaligned");
            }

            var onStack = Stack.Top(2);

            if (onStack == null)
            {
                throw new SigilException("StoreObject expects two values on the stack", Stack);
            }

            var val = onStack[0];
            var addr = onStack[1];

            if (TypeOnStack.Get(valueType) != val)
            {
                throw new SigilException("StoreObject expected a " + valueType + " to be on the stack, found " + val, Stack);
            }

            if (!addr.IsPointer && !addr.IsReference && addr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("StoreObject expected a reference, pointer, or native int; found " + addr, Stack);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Stobj, pop: 2);
        }
    }
}
