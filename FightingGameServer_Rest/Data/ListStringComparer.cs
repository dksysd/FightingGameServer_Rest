using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FightingGameServer_Rest.Data;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ListStringComparer() : ValueComparer<List<string>>(
    (l1, l2) => l1 != null && l2 != null && l1.SequenceEqual(l2), // 내용 기반 비교
    l => l.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // 내용 기반 해시 코드
    l => l.ToList() // Deep copy (깊은 복사) - 변경 추적을 위해
);