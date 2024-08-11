<template>
  <div class="w-screen">

    

    <div class="flex flex-col" style="width: 50%; margin:auto auto;">
      <div>
        <h1 class="text-2xl">Clients</h1>
        <div class="m-4">
          <router-link to="/Clients" custom v-slot="{ navigate }">
          <ButtonBase @click="navigate">New Client</ButtonBase>
        </router-link>
        </div>
       
        <table v-if="state.clients.length > 0" class="m-4">
          <thead>
            <th>ID</th>
            <th>Name</th>
            <th>Address</th>
            <th>Email</th>
            <th>Edit Link</th>
          </thead>
          <tbody>
            <tr v-for="client in state.clients" :key="client.id">
              <td>{{client.id}}</td>
              <td>{{client.name}}</td>
              <td>{{client.address}}</td>
              <td>{{client.email}}</td>

              <td>
                <router-link :to="{ name: 'GetClient', params: { id: client.id }}">
                  Open
                </router-link>
              </td>
              
            </tr>
          </tbody>
        </table>
        <div class="w-full flex justify-center"  v-if="state.clients.length == 0">
          No clients
        </div>

        
      </div>
      

      <h1 class="text-2xl">Invoices</h1>
      <div class="flex flex-row">
        <div class="flex justify-center items-center" >Client: </div>
        <SelectBase class="border border-black border-1" v-model="state.selectedClientID">
          <SelectTrigger class="w-[180px] ">
            <SelectValue :value="state.selectedClientID.toString()" placeholder="Select a Client" class="flex justify-space-between"/>
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              <SelectLabel>Clients</SelectLabel>
              <SelectItem v-for="client in state.clients" :key="client.id" :value="client.id.toString()"  > 
                {{ client.name }} - {{ client.email }} - {{ client.address }}
              </SelectItem>
            </SelectGroup>
          </SelectContent>
        </SelectBase>
      </div>

      <div>
        <form  v-if="state.clients.length != 0 && state.selectedClientID != 0" @submit.prevent>
          <LabelBase for="invoices">Create a new invoice</LabelBase>
          <InputBase type="text" name="invoices" v-model="state.description" placeholder="Description" />
          <ButtonBase @click="createInvoice">Create Invoice</ButtonBase>
        </form>

        <hr />

        <table class="m-4"> 
          <thead>
            <th>ID</th>
            <th>Description</th>
            <th>Invoice Link</th>
            <th>Total value</th>
            <th>Total Billed Amount</th>
          </thead>
          <tbody>
            <tr v-for="invoice in state.invoices" :key="invoice.id">
              <td>{{invoice.id}}</td>
              <td>{{invoice.description}}</td>
              <td>
                <router-link :to="{ name: 'Invoice', params: { id: invoice.id}, query: {clientID: state.selectedClientID.toString()}}">
                  Open
                </router-link>
              </td>
              <td>{{invoice.totalValue}}</td>
              <td>{{invoice.totalBillableValue}}</td>
              
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    

    
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, reactive, ref, watch } from 'vue';
import { Button as  ButtonBase} from '@/components/ui/button';
import { Input as InputBase } from '@/components/ui/input';
import { Label as LabelBase } from '@/components/ui/label';
import api from '@/api/invoice-application-api';
import {
  Select as SelectBase,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

export default defineComponent({
  name: 'InvoicesComponent',
  components: {
    LabelBase,
    InputBase,
    SelectBase,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectLabel,
    SelectTrigger,
    SelectValue,
    ButtonBase,
  },
  setup() {

    const getInvoices = async () =>{
      const response = await api.get("/invoices");
      if(response.status == 200) {
        state.invoices = response.data.invoices
      }
    }

    const getClients = async () =>{
      const response = await api.get("/Clients");
      if(response.status == 200) {
        state.clients = response.data;
      }
    }

    const createInvoice = async () => {
        const response = await api.post("/invoices",
            JSON.stringify({
              clientId: state.selectedClientID,
              description: state.description
            })
        );
    }

    const isEmpty = (object:any) => object === null || object === undefined || object.length === 0;
    
    const selectClient = (client: any) => {
      state.selectedClientID = Number(client.id);
    };


    onMounted(() => {
      getClients();
      getInvoices();
    });

    const state = reactive({
      invoices: [],
      description: "",
      clients: [],
      selectedClientID: 0
    });

    return {state, selectClient,createInvoice, getClients, getInvoices, isEmpty};
  }
});
</script>
