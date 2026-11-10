document.addEventListener("DOMContentLoaded", () => {
    const fileInput = document.getElementById("dataFile");
    const fileName = document.getElementById("file-name");
    const dropzone = document.getElementById("upload-dropzone");
    const form = document.getElementById("upload-form");
    const submitButton = document.getElementById("submit-upload");

    if (!fileInput || !fileName || !dropzone) {
        return;
    }

    const updateSelectedFile = () => {
        const file = fileInput.files?.[0];
        fileName.textContent = file ? file.name : "Nenhum arquivo selecionado";
        fileName.classList.toggle("has-file", Boolean(file));
        dropzone.classList.toggle("is-selected", Boolean(file));
    };

    fileInput.addEventListener("change", updateSelectedFile);

    ["dragenter", "dragover"].forEach((eventName) => {
        dropzone.addEventListener(eventName, (event) => {
            event.preventDefault();
            dropzone.classList.add("is-selected");
        });
    });

    ["dragleave", "drop"].forEach((eventName) => {
        dropzone.addEventListener(eventName, (event) => {
            event.preventDefault();
            dropzone.classList.toggle("is-selected", Boolean(fileInput.files?.length));
        });
    });

    dropzone.addEventListener("drop", (event) => {
        if (!event.dataTransfer?.files?.length) {
            return;
        }

        fileInput.files = event.dataTransfer.files;
        updateSelectedFile();
    });

    form?.addEventListener("submit", () => {
        if (fileInput.files?.length && submitButton) {
            submitButton.disabled = true;
            submitButton.innerHTML = 'Lendo arquivo <span aria-hidden="true">…</span>';
        }
    });
});
