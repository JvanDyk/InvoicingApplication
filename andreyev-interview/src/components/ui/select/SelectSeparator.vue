<script setup lang="ts">
import { type HTMLAttributes, computed } from 'vue'
import { SelectSeparator, type SelectSeparatorProps } from 'radix-vue'
import { cn } from '@/lib/utils'

const props = defineProps<SelectSeparatorProps & { className?: HTMLAttributes['class'] }>()

const delegatedProps = computed(() => {
  const { className: _, ...delegated } = props

  return delegated
})
</script>

<template>
  <SelectSeparator v-bind="delegatedProps" :class="cn('-mx-1 my-1 h-px bg-muted', props.class)" />
</template>
