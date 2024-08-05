<template>
    <router-link :to="{ name: 'Invoices' }">Back</router-link>

    <h2>Client Details</h2>
    <div class="client">
        <form @submit.prevent="onSubmit" class="max-w-2xl border-2 border-black rounded-md p-4">
            <!-- <form @submit.prevent class="max-w-2xl border-2 border-black rounded-md p-4"> -->
            <FormField name="name">
                <FormItem>
                    <FormLabel>Name:</FormLabel>
                    <FormControl>
                        <InputBase 
                            :value="values.name" 
                            @input="setFieldValue('name', $event.target.value)" 
                            class="form-field" 
                        />
                    </FormControl>
                    <FormDescription />
                    <FormMessage v-if="errors.name">{{ errors.name }}</FormMessage>
                </FormItem>
            </FormField>

            <FormField name="address">
                <FormItem>
                    <FormLabel>Address:</FormLabel>
                    <FormControl>
                        <InputBase 
                            :value="values.address" 
                            @input="setFieldValue('address', $event.target.value)" 
                            class="form-field" 
                        />
                    </FormControl>
                    <FormDescription />
                    <FormMessage v-if="errors.address">{{ errors.address }}</FormMessage>
                </FormItem>
            </FormField>

            <FormField name="email">
                <FormItem>
                    <FormLabel>Email:</FormLabel>
                    <FormControl>
                        <InputBase 
                            :value="values.email" 
                            @input="setFieldValue('email', $event.target.value)" 
                            class="form-field" 
                        />
                    </FormControl>
                    <FormDescription />
                    <FormMessage v-if="errors.email">{{ errors.email }}</FormMessage>
                </FormItem>
            </FormField>

            <div class="flex flex-row justify-end">
                <ButtonBase v-if="!isEmpty(clientId)" type="submit" variant="destructive" @click="deleteClient">Delete</ButtonBase>
                <ButtonBase v-if="isEmpty(clientId)" type="submit" @click="createClient">Create</ButtonBase>
                <ButtonBase v-if="!isEmpty(clientId)" type="submit"  @click="updateClient">Update</ButtonBase>
            </div>
        </form>
    </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';
import { Button as ButtonBase } from '@/components/ui/button';
import { Input as InputBase } from '@/components/ui/input';
import {
    FormControl,
    FormDescription,
    FormField,
    FormItem,
    FormLabel,
    FormMessage
} from '@/components/ui/form';
import * as z from 'zod';
import { toTypedSchema } from '@vee-validate/zod';
import { useForm } from 'vee-validate';
import api from '@/api/invoice-application-api';
import { useRoute, useRouter } from 'vue-router';

export default defineComponent({
    name: 'ClientComponent',
    components: {
        ButtonBase,
        FormControl,
        FormDescription,
        FormField,
        FormItem,
        FormLabel,
        FormMessage,
        InputBase
    },
    setup() {
        const isEmpty = (object:any) => object === null || object === undefined || object.length === 0;
    
        const clientId = ref(null); // Reactive property for client entity ID

        const getClient = async (id: string) => {
            try {
                const response = await api.get(`/clients/${id}`);
                if (response.status === 200) {
                    clientId.value = response.data.id;
                    setFieldValue('name', response.data.name);
                    setFieldValue('address', response.data.address);
                    setFieldValue('email', response.data.email);
                }
            } catch (error) {
                console.error("Error creating client:", error);
            }
        }
        const router = useRouter();
        const route = useRoute();

        // Check if the current route has a path that includes /:id
        const hasIdParam = route.matched.some((record: { path: string|string[]; }) => record.path.includes('/:id'));
        if(hasIdParam && route.params.id) {
            const id = route.params.id;
            getClient(id as string);
        }
        const formSchema = toTypedSchema(z.object({
            name: z.string().min(2, "Name is required and must be at least 2 characters long.").max(50, "Name must be at most 50 characters long."),
            address: z.string().min(5, "Address is required and must be at least 5 characters long.").max(80, "Address must be at most 80 characters long."),
            email: z.string().email("Invalid email format.").min(2, "Email is required.").max(50, "Email must be at most 50 characters long.")
        }));

        const { handleSubmit, values, errors, validateField, setFieldValue } = useForm({
            validationSchema: formSchema,
            initialValues: {
                name: '',
                address: '',
                email: ''
            }
        });

        const onSubmit = handleSubmit(async (values) => {
            // Validate each field individually
            const nameValid = await validateField('name');
            const addressValid = await validateField('address');
            const emailValid = await validateField('email');

            // Check if all fields are valid
            if (!nameValid || !addressValid || !emailValid) {
                console.log("Some fields are invalid.");
                return;
            }
        });

        const deleteClient = async () => {
            onSubmit();
            try {
                const response = await api.delete(`/clients/${clientId.value}`);
                if (response.status === 200) {
                    router.push({ name: 'Invoices' });
                }
            } catch (error) {
                console.error("Error creating client:", error);
            }
        }

        const updateClient = async () => {
            onSubmit();
            try {
                const response = await api.put(`/clients/${clientId.value}`, JSON.stringify({...values, id: clientId.value}));
                if (response.status === 200) {
                    router.push({ name: 'Invoices' });
                }
            } catch (error) {
                console.error("Error creating client:", error);
            }
        }

        const createClient = async () => {
            onSubmit();
            try {
                const response = await api.post("/clients", JSON.stringify(values));
                if (response.status === 200) {
                    router.push({ name: 'Invoices' });
                }
            } catch (error) {
                console.error("Error creating client:", error);
            }
        }
       
        return { onSubmit, values, errors, setFieldValue, clientId, createClient, deleteClient, updateClient, isEmpty };
    }
});
</script>