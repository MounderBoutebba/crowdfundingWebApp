import { firstUpperPipe } from './firstUpperPipe'

describe('pipe first letter to uppercase', () => {

    let pipe: firstUpperPipe;
    beforeEach(() => {
        pipe = new firstUpperPipe();
    });
    afterEach(() => {
        pipe = null;
    });
    it('pipe should be defined ', () => {
        expect(pipe).toBeDefined();
    })
    it("should transform string 'test' to upper first 'Test'", () => {
        expect(pipe.transform('test')).toEqual('Test');
    });
});