<template>
  <body class="bg-light">
    <!-- container -->
    <div class="container d-flex flex-column">
      <div class="row align-items-center justify-content-center g-0 min-vh-100">
        <div>
          <!-- Card -->
          <div class="card smooth-shadow-md">
            <!-- Card body -->
            <div class="card-body p-6">
              <div class="mb-4">
                <p class="mb-6 text-sm-center">Azure Blob Uploader</p>
              </div>

              <div class="mb-4">
                <input type="file" class="form-control" @change="handleImageChange" ref="fileInput">
              </div>
              <ImageMessage :message="errorMessageOversize"/>

              <div class="mb-4">
                <label for="textarea-input" class="form-label">Description</label>
                <textarea 
                    class="form-control"
                    id="textarea-input"
                    rows="3"
                    v-model="text">
                </textarea>
                <div>Characters left: {{ remainingChars }}</div>
              </div>

              <div class="d-lg-flex justify-content-between align-items-center mb-4">
                <div class="form-check custom-checkbox">
                  <input v-model="allowImageOverwrite" type="checkbox" class="form-check-input" id="allowImageOverwrite">
                  <label class="form-check-label" for="allowImageOverwrite">Allow image overwrite with the same name</label>
                </div>
              </div>

              <!-- Button -->
              <div>
                <div class="d-grid">
                  <button 
                  @click.prevent="saveImage" 
                  class="btn btn-primary"
                  :disabled="disabledButton">
                  Upload image
                  </button>
                </div>
              </div>

              <ImageMessage :message ="responseMessage"/>
            </div>
          </div>
        </div>
      </div>
    </div>
  </body>
</template>

<script>
import * as ApiCalls from "../helpers/ApiCalls.js";
import ImageMessage from "./ImageMessage.vue";
import { ref } from "vue";

export default {
  name: "ImageUploader",

  components: { ImageMessage },

  setup() {
    const fileInput = ref(null);
    return { fileInput };
  },

  data() {
    return {
      image: null,
      errorMessageOversize: "",
      text: "",
      allowImageOverwrite: false,
      maxLength: 300,
      disabledButton: false,
      responseMessage: ""
    }
  },

  methods: {
    async saveImage() {
      const formData = new FormData();
      formData.append("fileName", this.image.name);
      formData.append("description", this.text);
      formData.append("image", this.image); 
      formData.append("allowImageOverwrite", this.allowImageOverwrite);
      
      const result = await ApiCalls.uploadImageToAzureBlob(formData);
      this.responseMessage = result.responseMessage;
    },

    handleImageChange(event) {
      const image = event.target.files[0];
      let size = 0;

      if (image) {
        this.image = image;
        size = image.size;
        const validSize = this.validateImageSize(size);
        this.disabledButton = !validSize;

      } else {
        this.image = null;
        this.disabledButton = true;
      }
    },

    validateImageSize(size){
      const maxSize = 2 * 1024 * 1024; // 2 MB
      if(size > maxSize){
        this.errorMessageOversize = "Image size must not exceed 2 MB";
        return false;
      }
      else{
        this.errorMessageOversize = "";
        return true;
      }
    },

    updateText(text){
      this.text = text;
    }
  },

  computed: {
    remainingChars() {
      return this.maxLength - this.text.length;
    },
  },
};
</script>
