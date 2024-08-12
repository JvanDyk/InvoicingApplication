export interface IInvoice {
  id: number;
  discount: number;
  description: string;
  totalValue: number;
  totalBillableValue: number;
  totalNumberLineItems: number;
}

