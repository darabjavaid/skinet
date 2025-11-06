import {nanoid} from "nanoid";
export type CartType ={
  id: string;
  items: CartItem[];
  clientSecret?: string;
  paymentIntentId?: string;
  deliveryMethodId?: number;
}

export type CartItem = {
  productId: number;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
  type: string;
  brand: string;
}


export class Cart implements CartType{
  id = nanoid();
  items: CartItem[] = [];
  clientSecret?: string;
  paymentIntentId?: string;
  deliveryMethodId?: number;
}
