const apiUrl = process.env.API_URL;

export async function uploadImageToAzureBlob(body) {
    let responseMessage = "";

    try {
        let response = await fetch(`${apiUrl}/api/images`, {
            method: 'POST',
            body: body
        });
          
        if (response.status === 200) {
            responseMessage = "Image uploaded successfully"
        } else {
            responseMessage = await parseErrorMessage(response);     
        }        
    }
    catch (error) {
        responseMessage = error.message;
    }

    return {
        responseMessage: responseMessage,
    }
}

async function parseErrorMessage(response){    
    if (response.status === 500) {
        return "Internal Server Error."
    }

    const contentResponse = await response.json();

    if (response.status === 400) {        
        return parsValidationErrors(contentResponse);        
    }
    return contentResponse;
}

function parsValidationErrors(errors) {
    let errorMessage = "";

    for (let key in errors) {
        if (Object.prototype.hasOwnProperty.call(errors, key)) {
            errorMessage += errors[key][0] + '\n';
        }
    }
    return errorMessage;
}