// src/views/Inventory/utils/formatters.ts
export function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}

export function stockStatusLabel(stock: number) {
  if (stock === 0) return 'หมด'
  if (stock <= 10) return 'ใกล้หมด'
  return 'ปกติ'
}

export function stockStatusClass(stock: number) {
  if (stock === 0)
    return 'inline-flex rounded-full bg-rose-100 px-2.5 py-1 text-[11px] font-semibold text-rose-700'
  if (stock <= 10)
    return 'inline-flex rounded-full bg-amber-100 px-2.5 py-1 text-[11px] font-semibold text-amber-700'
  return 'inline-flex rounded-full bg-emerald-100 px-2.5 py-1 text-[11px] font-semibold text-emerald-700'
}
