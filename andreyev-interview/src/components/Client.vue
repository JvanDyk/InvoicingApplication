<template>
    <Dialog v-if="true" style="flex: 1; z-index: 9;">
        <div class="client" contenteditable="true">
            <Form class="form" label-position="top" label-width="100px">
            <FormItemBase label="Id">
                <InputBase v-model="state.Id" disabled />
            </FormItemBase>
            <FormItem label="Name">
                <InputBase v-model="state.Name" />
            </FormItem>
            <FormItem label="Email">
                <InputBase v-model="state.Email" />
            </FormItem>
            <FormItem label="Address">
                <InputBase v-model="state.Address" />
            </FormItem>
            </Form>

            <div class="buttons">
            <ButtonBase @click="saveClient">Save</ButtonBase>
            <ButtonBase @click="deleteClient">Delete</ButtonBase>
            </div>
        </div>
    </Dialog>
</template>

<script lang="ts">
import { defineComponent, reactive, ref, toRefs } from 'vue';
import { Button as ButtonBase} from '@/components/ui/button';
import { Input as InputBase } from '@/components/ui/input';
import {
  // FormControl,
  // FormDescription,
  // FormField,
  FormItem as FormItemBase,
  // FormLabel,
  // FormMessage
} from '@/components/ui/form'

export default defineComponent({
  name: 'ClientComponent',
  components: {
    ButtonBase,
    InputBase,
    FormItemBase,
  },
  props: {
    showDialog: Boolean
  },
  emits: ['update:showDialog'],
  setup(props, { emit }) {
    const clientState = reactive({
      Id: 0,
      Name: '',
      Email: '',
      Address: ''
    });
    const showDialogComp  = ref(props.showDialog);
    const toggleDialog = () => {
        showDialogComp.value = !showDialogComp.value;
      emit('update:showDialog', showDialogComp.value);
    };

    function saveClient() {
      fetch("http://localhost:5000/clients", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          Id: clientState.Id,
          Name: clientState.Name,
          Email: clientState.Email,
          Address: clientState.Address
        })
      }).then(() => {
        window.location.reload();
      });
    }

    // function updateClient() {
    //   fetch(`http://localhost:5000/clients/${state.Id}`, {
    //     method: "PUT",
    //     headers: {
    //       "Content-Type": "application/json"
    //     },
    //     body: JSON.stringify({
    //       Id: state.Id,
    //       Name: state.Name,
    //       Email: state.Email,
    //       Address: state.Address
    //     })
    //   }).then(() => {
    //     window.location.reload();
    //   });
    // }

    function deleteClient() {
      fetch(`http://localhost:5000/clients/${clientState.Id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json"
        }
      }).then(() => {
        window.location.reload();
      });
    }

    // eslint-disable-next-line vue/no-dupe-keys
    return { ...toRefs(clientState), showDialog: showDialogComp, toggleDialog, saveClient, deleteClient }
  }
});
</script>

<style scoped>
.client {
  z-index: 9;
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
}

.form {
  width: 50%;
}

.buttons {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}
</style>
