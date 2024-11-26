<template>
    <div class="mb-4 mt-4">
        <div v-if="message" class="alert alert-primary" role="alert"> {{ message }} </div>
    </div>  
</template>

<script>
export default {

    props: {
        message: String 
    }
}
</script>
