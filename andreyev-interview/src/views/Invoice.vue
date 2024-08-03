<template>
  <div class="about">
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
        <th>Billable</th>
      </thead>
      <tbody>
        <tr v-for="item in state.lineItems.lineItem" :key="item.id">
          <td>{{item.id}}</td>
          <td>{{item.description}}</td>
          <td>{{item.quantity}}</td>
          <td>{{item.cost}}</td>
           <td>  
            <input type="checkbox" :id="item.id" @change="handleBillableStatus($event)" :name="item.invoiceId" :value="item.isBillable" :checked = "item.isBillable ? true : false" >
            
          </td>
        </tr>
      </tbody>
    </table>

<div class='text-right'><strong>Total Value : </strong> {{state.lineItems.grandTotal}}</div>
<div class='text-right'><strong>Total Billable Value : </strong> {{state.lineItems.totalBillableValue}}</div>

    <form @submit.prevent>
      <h4>Create Line Item</h4>
      <input type="text" name="description" placeholder="Description" v-model="state.description" />
      <input type="number" name="quantity" placeholder=   "Quantity" v-model="state.quantity" />
      <input type="number" name="cost" placeholder="Cost" v-model="state.cost" />
      Billable: <input type="checkbox" name="isbillable"  v-model="state.isbillable" />
      <br/>
      <br/>
      <button @click="createLineItem">Create Invoice</button>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent, onMounted, reactive} from "vue";

export default defineComponent({
  name: "InvoiceComponent",
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  methods:{
    handleBillableStatus: function (event : any){
    
        let {value,id,name} = event.target ;
       // console.log(event.target.value);
       // console.log(event.target.id);

  let newValue = false;

        if(value == "true"){
          newValue = false;
        }else if(value == "false"){
          newValue = true;
        }

       // console.log(newValue);


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
      invoiceId: props.id
    })

    function fetchLineItems() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(lineItems => (state.lineItems = lineItems))
      })
    }

    function createLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
          isBillable: state.isbillable
        })
      }).then(fetchLineItems)
    }

    onMounted(fetchLineItems)

    return {state, createLineItem}
  }
})
</script>

