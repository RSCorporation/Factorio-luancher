factorioModUpdater = {
    dir: "D:/Users/dimava/Documents/MEGA/factorio-mods",
    get folder() {
        if (this.readFolder.list)
            return this.readFolder.list;
        return this.readFolder();
    },
    readFolder() {
        return this.readFolder.list = fs.readdirSync(`${this.dir}/0.16`);
    },
    async getList(n=9999) {
        let r = await fetch(`https://mods.factorio.com/api/mods?page_size=${n}`,{credentials:'include'});
        let j = await r.json();
        let list = this.list = this.listFrom(j.results);
        if (n == 9999)
            this.fullList = list;
        this.onListFetched(list);
        return list;
    },
    listFrom(results) {
        let fmu = this;
        let proto = {
            get factorioVersion() {
                return +this.latest_release.info_json.factorio_version.split('.')[1];
            },
            get fileName() {
                return this.latest_release.file_name;
            },
            get path() {
                return `${fmu.dir}/0.${this.factorioVersion}/${this.fileName}`;
            },
            get exists() {
                return fs.existsSync(this.path);
            },
            get href() {
                return `https://mods.factorio.com${this.latest_release.download_url}`;
            },
            get age() {
                let stat = fs.statSync(this.path);
                let age = new Date() - stat.mtime;
                return age / 1000;
            },
            async download() {
                await fmu.logIn();

                let f = await fetch(this.href,{credentials:'include'});
                if (f.url.match('login')) {
                    return console.log('not logged in!');
                }
                if (f.status !== 200) {
                    return console.log('bad status!');
                }
                let b = await f.arrayBuffer();
                fs.writeFileSync(this.path, new Buffer(b));
                console.info(`mod %o downloaded!`, this.fileName);
                return true;
            },
            findAlike() {
                let folder = fmu.folder;
                let name = this.name + '_';
                return folder.filter(e=>e.indexOf(name) != -1)//
                .filter(e=>e.slice(name.length).match(/^\d+/))//
                .filter(e=>e != this.fileName);
            },
        }
        results.forEach(e=>e.__proto__ = proto);
        return this.list = results;

    },
    get unloaded() {
        let v16 = this.list.filter(e=>e.factorioVersion == 16);
        let unex = v16.filter(e=>!e.exists);

        return unex

    },
    findAlike() {
        return [].concat(...this.list.map(e=>e.findAlike()))
    },
    //     loggedIn: true,
    async logIn() {
        if (this.loggedIn)
            return;
        let w = open('http://mods.factorio.com');
        await new Promise(r=>w.onload=r);
        if (!w.document.querySelector('.user-session span')) {
            w.location.href = 'http://mods.factorio.com/login';
            this.onLoginRequired();
            await new Promise(r=>w.onload=r);
            await new Promise(r=>{w.onload=r,w.onclose=r});
        }
        !w.closed && w.close();
        this.loggedIn = true;
    },
    async downloadNew() {
        let fmu = this;
        if(!this.list) await this.getList();
        let todo = this.unloaded;
        let result = todo.pmap(async function(mod, i, a) {
            fmu.onModDownloading(mod, {
                i,
                n: a.length
            });
            await mod.download();
            fmu.onModDownloaded(mod, {
                i,
                n: a.length
            });
        });

    },
    async removeOld() {
        let fmu = this;
        if(!this.list) await this.getList();
        let todo = this.list.filter(e=>e.findAlike().length);
        todo.forEach(function(mod, i, a) {
            for (let oldName of mod.findAlike()) {
                fs.rename(`${fmu.dir}/0.16/${oldName}`, `${fmu.dir}/0.16/outdated/${oldName}`, ()=>fmu.onModDeleted(mod, oldName, {
                    i,
                    n: a.length
                }));
                ;
            }
        });

    },
    onLoginRequired() {},
    onListFetched(list) {},
    onModDownloading(mod, state) {},
    onModDownloaded(mod, state) {},
    onModDeleted(mod, oldName) {},

}
fmu = factorioModUpdater

commands.fmu_bind = async function(msg) {
    with (msg) {
        let fmu = factorioModUpdater;
        //         function answer(...a) {
        //             return console.log(...a);
        //         }

        fmu.onLoginRequired = ()=>answer('Logging in required. <@295973981234790400>?');
        fmu.onListFetched = ()=>answer(`\`\`\`Mod list downloaded.
        entries: ${fmu.list.length}
        v16: ${fmu.list.filter(e=>e.factorioVersion == 16).length}
        unloaded: ${fmu.unloaded.length}
        outdated: ${fmu.findAlike().length}\`\`\``);
        fmu.onModDownloading = (mod,state)=>answer(`Mod #${state.i + 1}/${state.n} \`${mod.name}\` \`${mod.name}\` has started loading.`);
        fmu.onModDownloaded = (mod,state)=>answer(`Mod #${state.i + 1}/${state.n} \`${mod.name}\` has beed loaded.`);
        fmu.onModDeleted = (mod,oldName,state)=>answer(`Outdated version of mod #${state.i + 1}/${state.n} \`${mod.fileName}\`, \`${oldName}\`, has been moved to \`outdated\` folder.`);

    }
}
commands.fmu_bind(message);

commands.fmu_list = function(msg) {
        fmu.getList();
}
commands.fmu_load = function() {
    fmu.downloadNew();
}
commands.fmu_del = function() {
    fmu.removeOld();
}