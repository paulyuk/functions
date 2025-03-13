const axios = require('axios');

const orchestratorUrl = 'http://localhost:7071/api/orchestrators/chainingOrchestrator';

async function startOrchestration() {
    try {
        const response = await axios.post(orchestratorUrl, {});
        return response.data;
    } catch (error) {
        console.error('Error starting orchestration:', error);
        throw error;
    }
}

async function getStatus(statusQueryGetUri) {
    try {
        const response = await axios.get(statusQueryGetUri);
        return response.data;
    } catch (error) {
        console.error('Error getting status:', error);
        throw error;
    }
}

async function main() {
    const response = await startOrchestration();
    const instanceId = response.id;
    const statusQueryGetUri = response.statusQueryGetUri;
    console.log(`Orchestration started with ID: ${instanceId}`);

    let isCompleted = false;
    while (!isCompleted) {
        const status = await getStatus(statusQueryGetUri);
        console.log(`Status: ${status.runtimeStatus}`);
        if (status.runtimeStatus === 'Completed' || status.runtimeStatus === 'Failed' || status.runtimeStatus === 'Terminated') {
            isCompleted = true;
            console.log('Final result:', status.output);
        } else {
            await new Promise(resolve => setTimeout(resolve, 5000)); // Wait for 5 seconds before checking the status again
        }
    }
}

main().catch(error => console.error('Error in main:', error));
