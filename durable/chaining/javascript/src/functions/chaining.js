const { app } = require('@azure/functions');
const df = require('durable-functions');

const activityName = 'chaining';

df.app.orchestration('chainingOrchestrator', function* (context) {
    const outputs = [];

    context.log(`Called activity '${activityName}' with 'Tokyo'.`);
    outputs.push(yield context.df.callActivity(activityName, 'Tokyo'));
    context.log(`Outputs updated to '${outputs}'`);

    context.log(`Called activity '${activityName}' with 'Seattle'.`);
    outputs.push(yield context.df.callActivity(activityName, 'Seattle'));
    context.log(`Outputs updated to '${outputs}'`);

    context.log(`Called activity '${activityName}' with 'Cairo'.`);
    outputs.push(yield context.df.callActivity(activityName, 'Cairo'));
    context.log(`Outputs updated to '${outputs}'`);

    return outputs;
});

df.app.activity(activityName, {
    handler: (input) => {
        return `Hello, ${input}`;
    },
});

app.http('chainingHttpStart', {
    route: 'orchestrators/{orchestratorName}',
    extraInputs: [df.input.durableClient()],
    handler: async (request, context) => {
        const client = df.getClient(context);
        const body = await request.text();
        const instanceId = await client.startNew(request.params.orchestratorName, { input: body });

        context.log(`Started orchestration with ID = '${instanceId}'.`);

        return client.createCheckStatusResponse(request, instanceId);
    },
});