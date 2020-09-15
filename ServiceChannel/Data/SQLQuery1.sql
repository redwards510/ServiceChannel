select i.Date, SUM(i.Count), SUM(i.NewCases)

from County c
join Infections i on c.id = i.countyid
where c.State = 'Washington'
and i.Date >= '3/3/2020' and i.date <= '3/5/2020'
group by i.Date

select i.Date, i.Count, i.NewCases

from County c
join Infections i on c.id = i.countyid
where c.State = 'Washington' and c.name = 'King'
and i.Date >= '3/3/2020' and i.date <= '3/5/2020'
group by i.Date