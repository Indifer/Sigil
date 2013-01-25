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
        public void LoadObject<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            LoadObject(typeof(ValueType), isVolatile, unaligned);
        }

        public void LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            if (!valueType.IsValueType)
            {
                throw new ArgumentException("valueType must be a ValueType", "valueType");
            }

            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4", "unaligned");
            }

            var onStack = Stack.Top();
            if (onStack == null)
            {
                throw new SigilException("LoadObject expected a value on the stack, but it was empty", Stack);
            }

            var ptr = onStack[0];

            if (!ptr.IsReference && !ptr.IsPointer && ptr != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("LoadObject expected a reference, pointer, or native int; found " + ptr, Stack);
            }

            if (isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Ldobj, TypeOnStack.Get(valueType), pop: 1);
        }
    }
}
