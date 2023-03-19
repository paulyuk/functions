const { app } = require('@azure/functions');

app.http('http', {
    methods: ['GET', 'POST'],
    authLevel: 'anonymous',
    handler: async (request, context) => {
        context.log(`Http function processed request for url "${request.url}"`);

        const name = request.query.get('name') || (request.body && request.body.name) || 'world';

        return { body: `Hello, ${name}!` };
    }
});
