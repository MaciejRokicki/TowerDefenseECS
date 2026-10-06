using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace TD.Shared
{
    [NativeContainer]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeMinHeap<T> : IDisposable where T : unmanaged, IComparable<T>
    {
        [NativeDisableUnsafePtrRestriction]
        private UnsafeList<T>* m_Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;

        private static readonly SharedStatic<int> s_SafetyId =
            SharedStatic<int>.GetOrCreate<NativeMinHeap<T>>();
#endif

        public NativeMinHeap(int initialCapacity, Allocator allocator)
        {
            ValidateConstructor(initialCapacity, allocator);
            m_Data = UnsafeList<T>.Create(initialCapacity, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = CollectionHelper.CreateSafetyHandle(allocator);
            CollectionHelper.SetStaticSafetyId<NativeMinHeap<T>>(
                ref m_Safety, ref s_SafetyId.Data);

            if (UnsafeUtility.IsNativeContainerType<T>())
                AtomicSafetyHandle.SetNestedContainer(m_Safety, true);
#endif
        }

        public bool IsCreated => m_Data != null;

        public int Count
        {
            get
            {
                CheckReadAccess();
                return m_Data->Length;
            }
        }

        public bool IsEmpty => Count == 0;

        public int Capacity
        {
            get
            {
                CheckReadAccess();
                return m_Data->Capacity;
            }
        }

        public void EnsureCapacity(int capacity)
        {
            CheckReadWriteAccess();
            ValidateCapacity(capacity);

            if (capacity > m_Data->Capacity)
                m_Data->Capacity = capacity;
        }

        public void Push(T item)
        {
            CheckReadWriteAccess();
            m_Data->Add(item);
            SiftUp(m_Data->Length - 1, item);
        }

        public bool TryPush(T item)
        {
            CheckReadWriteAccess();
            if (m_Data->Length == m_Data->Capacity)
                return false;

            m_Data->AddNoResize(item);
            SiftUp(m_Data->Length - 1, item);
            return true;
        }

        public bool TryPeek(out T item)
        {
            CheckReadAccess();
            if (m_Data->Length == 0)
            {
                item = default;
                return false;
            }

            item = m_Data->Ptr[0];
            return true;
        }

        public bool TryPop(out T item)
        {
            CheckReadWriteAccess();
            int count = m_Data->Length;
            if (count == 0)
            {
                item = default;
                return false;
            }

            item = m_Data->Ptr[0];
            T last = m_Data->Ptr[count - 1];
            m_Data->Length = count - 1;

            if (count > 1)
                SiftDown(0, last);

            return true;
        }

        public void Clear()
        {
            CheckWriteAccess();
            m_Data->Clear();
        }

        public void CopyFrom(NativeArray<T> items)
        {
            CheckReadWriteAccess();
            int count = items.Length;
            if (count > m_Data->Capacity)
                m_Data->Capacity = count;

            m_Data->Clear();
            for (int index = 0; index < count; ++index)
                m_Data->AddNoResize(items[index]);

            for (int index = (count >> 1) - 1; index >= 0; --index)
                SiftDown(index, m_Data->Ptr[index]);
        }

        public void Dispose()
        {
            if (m_Data == null)
                return;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
            CollectionHelper.DisposeSafetyHandle(ref m_Safety);
#endif

            UnsafeList<T>.Destroy(m_Data);
            m_Data = null;
        }

        private void SiftUp(int index, T item)
        {
            T* data = m_Data->Ptr;
            while (index > 0)
            {
                int parentIndex = (index - 1) >> 1;
                T parent = data[parentIndex];
                if (item.CompareTo(parent) >= 0)
                    break;

                data[index] = parent;
                index = parentIndex;
            }

            data[index] = item;
        }

        private void SiftDown(int index, T item)
        {
            T* data = m_Data->Ptr;
            int count = m_Data->Length;
            while (index < (count >> 1))
            {
                int childIndex = (index << 1) + 1;
                int rightIndex = childIndex + 1;
                if (rightIndex < count && data[rightIndex].CompareTo(data[childIndex]) < 0)
                    childIndex = rightIndex;

                T child = data[childIndex];
                if (item.CompareTo(child) <= 0)
                    break;

                data[index] = child;
                index = childIndex;
            }

            data[index] = item;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckReadAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
            CheckCreated();
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckWriteAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
            CheckCreated();
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckReadWriteAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
            CheckCreated();
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckCreated()
        {
            if (m_Data == null)
                throw new InvalidOperationException("NativeMinHeap is not created.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void ValidateCapacity(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void ValidateConstructor(int capacity, Allocator allocator)
        {
            ValidateCapacity(capacity);
            if (allocator != Allocator.Temp &&
                allocator != Allocator.TempJob &&
                allocator != Allocator.Persistent)
            {
                throw new ArgumentException("Use Temp, TempJob or Persistent.", nameof(allocator));
            }
        }
    }
}
