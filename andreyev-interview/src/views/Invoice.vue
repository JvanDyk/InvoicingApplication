<template>
  <div class="invoice flex flex-col justify-center w-screen">
    <div style="width: 50%; margin:auto auto;">
      <router-link :to="{ name: 'Invoices' }">Back</router-link>

        <h2>Invoice Details</h2>

        <span>Invoice #{{$route.params.id}}</span>

        <h3>Line Items</h3>

        <table>
          <thead>
            <th>ID</th>
            <th>Description</th>
            <th>Quantity</th>
            <th>Cost</th>
            <th>Total</th>
            <th>Billable</th>
            <th>Action</th>
          </thead>
          <tbody>
            <tr v-for="item in state.lineItems" :key="item.id">
              <td>{{item.id}}</td>
              <td>{{item.description}}</td>
              <td>{{item.quantity}}</td>
              <td>{{item.cost}}</td>
              <td>{{ item.quantity * item.cost }}</td>
              <td>  
                <InputBase type="checkbox" :id="item.id" @change="handleBillableStatus($event)" :name="item.invoiceId" :value="item.isBillable" :checked = "item.isBillable ? true : false" /> 
              </td>
              <td>
                <ButtonBase @click="deleteLineItem(item.id)">Delete Item</ButtonBase>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="state.discount > 0" class='text-right'><strong>Discount : </strong> {{state.discount}}%</div>
        <hr/>
        <div class='text-right'><strong>Total Value : </strong> {{state.grandTotal}} </div>
        <div class='text-right'><strong>Total Billable Value : </strong> {{state.totalBillableValue}} </div>
        <div v-if="state.discount > 0" class='text-right'><strong>Final Total Billable Value : </strong> {{state.totalBillableValue * (100 - state.discount ) / 100}}</div>

        <form @submit.prevent class="mr-4 ml-4 mt-4 mb-4 border border-2 border-black rounded-md p-4">
          <h4>Create Line Item</h4>
          
          <FormLabel>Description:</FormLabel>
          <FormControl>
            <InputBase class="m-4" type="text" name="description"  v-model="state.description" />
          </FormControl>

          <FormLabel>Quantity:</FormLabel>
          <FormControl>
            <InputBase class="m-4" type="number" name="quantity" placeholder="Quantity" v-model="state.quantity" />
          </FormControl>

          <FormLabel>Cost:</FormLabel>
          <FormControl>
            <InputBase class="m-4" type="number" name="cost" placeholder="Cost" v-model="state.cost" />
          </FormControl>

          <FormLabel>Billable:</FormLabel>
          <FormControl>
            <InputBase type="checkbox" name="isbillable"  v-model="state.isbillable" />
          </FormControl>

          <br/>
          <br/>
          <div class="flex w-full flex-row justify-evenly">
            <ButtonBase @click="createLineItem">Add Line Item</ButtonBase>
            <ButtonBase variant="destructive" @click="deleteInvoice">Delete Invoice</ButtonBase>
            <ButtonBase @click="state.showDialog = true">Apply Coupon</ButtonBase>
          </div>
          
        </form>
        <div v-if="state.showDialog" class="dialog-overlay">
          <div class="dialog">
            <h2>Apply coupon discount</h2>
            <FormLabel>Discount (max 100):</FormLabel>
            <FormControl>
              <InputBase type="number" name="discount" placeholder="Discount" v-model.number="state.discount" :attrs="{max: 100}"/>          
            </FormControl>
            <div class="dialog-buttons">
              <button @click="applyCoupon">Apply Coupon</button>
              <button @click="state.showDialog = false">Cancel</button>
            </div>
          </div>
        </div>
        <h2>Invoice History</h2>

        <TableBase>
          <TableHeader>
            <TableRow>
              <TableHead>Created On</TableHead>
              <TableHead>Message</TableHead>
            </TableRow>
          
          </TableHeader>
          <TableBody>
            <TableRow v-for="item in state.invoiceHistory" :key="item.message">
              <TableCell>{{new Date(item.createdOn).toLocaleString(undefined, { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' })}}</TableCell>
              <TableCell>{{item.logMessage}}</TableCell>
            </TableRow>
          </TableBody>
        </TableBase>
    </div>
  </div>
</template>

<script lang="ts">
import {computed, defineComponent, onMounted, reactive, ref} from "vue";
import { Button as  ButtonBase} from "@/components/ui/button";
import { Input as InputBase } from "@/components/ui/input";
import { Table as TableBase, TableHead, TableBody, TableRow, TableCell, TableHeader} from "@/components/ui/table";
import api from '@/api/invoice-application-api';
import router from "@/router";
import { useRoute } from "vue-router";
export default defineComponent({
  name: "InvoiceComponent",
  components: {
    ButtonBase,
    InputBase,
    TableBase, TableHead, TableBody, TableRow, TableCell, TableHeader
  },
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  methods:{
    handleBillableStatus: function (event : any){
    
        let {value,id,name} = event.target ;

        let newValue = false;

        if(value == "true"){
          newValue = false;
        }else if(value == "false"){
          newValue = true;
        }



             fetch(`http://localhost:5000/invoices/Update/`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          lineItemId:parseInt(id),
          isBillable:newValue,
          invoiceId: parseInt(name)// pass the acctual selected InvoiceId 
        })
      }).then(() => {
        window.location.reload();
      });
    }
  },
  setup(props) {
    const state = reactive({
      lineItems: [],
      description: "",
      quantity: "0",
      cost: "0",
      isbillable: true,
      invoiceId: props.id,
      invoiceHistory: [],
      showDialog: false,
      discount: 0,
      grandTotal: 0,
      totalBillableValue: 0
    })

    const applyCoupon = async () => {
      if(state.discount > 100) {
        state.discount = 100;
      }
      const response = await api.get(`/invoices/discount/${props.id}?discount=${state.discount}`
      );
        
      if(response.status == 200) {
        fetchLineItems();
      }
      state.showDialog = false
    }


    const fetchLineItems = async () => {
      const response = await api.get("/invoices/"+ props.id);

      if(response.status == 200) {
        state.lineItems = response.data.lineItem;
        state.discount = response.data.discount;
        state.grandTotal = response.data.grandTotal;
        state.totalBillableValue = response.data.totalBillableValue;
      }
    }

    const createLineItem = async () => {
        const response = await api.post("/invoices/"+ props.id,
            JSON.stringify({
              description: state.description,
              quantity: Number(state.quantity),
              cost: Number(state.cost),
              isBillable: Boolean(state.isbillable) || state.isbillable
            })
        );
        
        if(response.status == 200) {
          fetchLineItems();
        }
    }

    const getInvoiceHistory = async () => {
        const response = await api.get("/invoices/history/"+ props.id);
        
        if(response.status == 200) {
          state.invoiceHistory = response.data
        }
    }

    const deleteLineItem = async (id: number) => {
      const response = await api.delete("/invoices/lineItemEntity/"+ id);
        
        if(response.status == 200) {
          fetchLineItems();
        }
    }

    const deleteInvoice = async () => {
        const response = await api.delete("/invoices/"+ props.id);
        
        if(response.status == 200) {
          router.push({ name: 'Invoices' });
        }
    }


    onMounted(() => {
      fetchLineItems();
      getInvoiceHistory();
    })

    return {state, createLineItem, getInvoiceHistory, deleteInvoice, deleteLineItem, applyCoupon}
  }
})
</script>

<style scoped>
.dialog-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}

.dialog {
  background-color: white;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
}

.dialog-buttons {
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}

.dialog-buttons button {
  margin-left: 10px;
}
</style>